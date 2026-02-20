const { app, BrowserWindow, dialog, ipcMain } = require('electron');
const fs = require('fs');
const path = require('path');

const statePath = () => path.join(path.dirname(app.getPath('exe')), 'config', 'lfe.config');

function ensureDir(p) { fs.mkdirSync(p, { recursive: true }); }

function parseLangFile(filePath) {
  const dict = {};
  if (!fs.existsSync(filePath)) return dict;
  const lines = fs.readFileSync(filePath, 'utf8').split(/\r?\n/);
  for (const raw of lines) {
    const line = raw.trim();
    if (!line || line.startsWith('#')) continue;
    const idx = line.indexOf('=');
    if (idx <= 0) continue;
    const key = line.slice(0, idx).trim();
    const value = line.slice(idx + 1).trim();
    if (!(key in dict)) dict[key] = value;
  }
  return dict;
}

function writeLangFile(filePath, dict) {
  const keys = Object.keys(dict).sort((a, b) => a.localeCompare(b));
  const lines = keys.map(k => `${k} = ${dict[k] ?? ''}`);
  ensureDir(path.dirname(filePath));
  fs.writeFileSync(filePath, lines.join('\n'), 'utf8');
}

function getLanguages(root) {
  if (!root || !fs.existsSync(root)) return [];
  return fs.readdirSync(root, { withFileTypes: true })
    .filter(d => d.isDirectory())
    .map(d => d.name)
    .sort((a, b) => a.localeCompare(b));
}

function getAvailableLangFiles(root) {
  const set = new Set();
  for (const lang of getLanguages(root)) {
    const dir = path.join(root, lang);
    if (!fs.existsSync(dir)) continue;
    for (const f of fs.readdirSync(dir)) if (f.toLowerCase().endsWith('.lang')) set.add(f);
  }
  return [...set].sort((a, b) => a.localeCompare(b));
}

function loadAll(root, fileName) {
  const languages = getLanguages(root);
  const data = {};
  for (const lang of languages) data[lang] = parseLangFile(path.join(root, lang, fileName));
  return { languages, data };
}

function saveAll(root, fileName, languages, data) {
  const allKeys = [...new Set(languages.flatMap(l => Object.keys(data[l] || {})))].sort((a, b) => a.localeCompare(b));
  for (const lang of languages) {
    const dict = {};
    for (const k of allKeys) dict[k] = (data[lang] || {})[k] ?? '';
    writeLangFile(path.join(root, lang, fileName), dict);
  }
}

function createLanguage(root, newLanguage, defaultLanguage) {
  const targetDir = path.join(root, newLanguage);
  if (fs.existsSync(targetDir)) throw new Error(`Language already exists: ${newLanguage}`);
  ensureDir(targetDir);

  const files = getAvailableLangFiles(root);
  const allLangs = getLanguages(root);

  for (const file of files) {
    let source = null;
    if (defaultLanguage) {
      const p = path.join(root, defaultLanguage, file);
      if (fs.existsSync(p)) source = parseLangFile(p);
    }

    if (!source) {
      const donor = allLangs.map(l => path.join(root, l, file)).find(fs.existsSync);
      if (donor) {
        const keys = Object.keys(parseLangFile(donor));
        source = Object.fromEntries(keys.map(k => [k, '']));
      } else {
        source = {};
      }
    }

    writeLangFile(path.join(targetDir, file), source);
  }
}

function normalizeTranslateUrl(url) {
  if (!url) return '';
  let normalized = String(url).trim().replace(/\/+$/, '');
  if (!normalized.toLowerCase().endsWith('/translate')) normalized += '/translate';
  return normalized;
}

function langCode(lang) {
  if (!lang) return 'en';
  const idx = lang.indexOf('-');
  return (idx > 0 ? lang.slice(0, idx) : lang).toLowerCase();
}

function extractError(body) {
  if (!body) return '(empty error message)';
  try {
    const parsed = JSON.parse(body);
    return parsed.error || parsed.message || body;
  } catch {
    return body.length > 300 ? body.slice(0, 300) + '...' : body;
  }
}

async function translateText({ text, source, target, baseUrl }) {
  const endpoints = [
    normalizeTranslateUrl(baseUrl) || 'https://libretranslate.com/translate',
    'https://libretranslate.de/translate',
    'https://translate.argosopentech.com/translate'
  ];

  const uniqueEndpoints = [...new Set(endpoints.filter(Boolean))];
  let lastError = null;

  for (const endpoint of uniqueEndpoints) {
    for (const src of [...new Set([langCode(source), 'auto'])]) {
      try {
        const res = await fetch(endpoint, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ q: text, source: src, target: langCode(target), format: 'text' })
        });

        const body = await res.text();
        if (!res.ok) {
          lastError = `${endpoint} - ${res.status} ${res.statusText}: ${extractError(body)}`;
          continue;
        }

        const json = JSON.parse(body);
        if (json.translatedText) return { translatedText: json.translatedText, usedEndpoint: endpoint };
        lastError = `${endpoint} - Response without translatedText`;
      } catch (err) {
        lastError = `${endpoint} - ${err.message}`;
      }
    }
  }

  try {
    const myMemoryUrl = `https://api.mymemory.translated.net/get?q=${encodeURIComponent(text)}&langpair=${langCode(source)}|${langCode(target)}`;
    const res = await fetch(myMemoryUrl);
    const body = await res.text();
    if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
    const json = JSON.parse(body);
    const translated = json?.responseData?.translatedText;
    if (translated) return { translatedText: translated, usedEndpoint: 'mymemory' };
  } catch (err) {
    if (!lastError) lastError = err.message;
  }

  throw new Error(lastError || 'No working translation endpoint found.');
}

const dirtyWindows = new Set();

async function createWindow() {
  const win = new BrowserWindow({
    width: 1450,
    height: 900,
    webPreferences: { preload: path.join(__dirname, 'preload.js') }
  });
  const wcId = win.webContents.id;

  win.on('close', (e) => {
    if (!dirtyWindows.has(wcId)) return;

    const result = dialog.showMessageBoxSync(win, {
      type: 'question',
      buttons: ['Abbrechen', 'Beenden ohne Speichern'],
      defaultId: 0,
      cancelId: 0,
      title: 'Ungespeicherte Änderungen',
      message: 'Es gibt ungespeicherte Änderungen. Wirklich beenden?'
    });

    if (result === 0) e.preventDefault();
  });

  win.on('closed', () => {
    dirtyWindows.delete(wcId);
  });

  await win.loadFile('index.html');
}

app.whenReady().then(() => {
  ipcMain.handle('choose-root-folder', async () => {
    const r = await dialog.showOpenDialog({ properties: ['openDirectory'] });
    return r.canceled || !r.filePaths.length ? null : r.filePaths[0];
  });

  ipcMain.handle('get-available-lang-files', (_, root) => getAvailableLangFiles(root));
  ipcMain.handle('load-all', (_, root, fileName) => loadAll(root, fileName));
  ipcMain.handle('save-all', (_, root, fileName, languages, data) => saveAll(root, fileName, languages, data));
  ipcMain.handle('create-language', (_, root, newLanguage, defaultLanguage) => createLanguage(root, newLanguage, defaultLanguage));

  ipcMain.handle('load-state', () => {
    const p = statePath();
    if (!fs.existsSync(p)) return { rootPath: '', selectedLangFileName: '', uiLanguage: 'de', translateUrl: 'https://libretranslate.com/translate' };
    return JSON.parse(fs.readFileSync(p, 'utf8'));
  });

  ipcMain.handle('save-state', (_, state) => {
    const p = statePath();
    ensureDir(path.dirname(p));
    fs.writeFileSync(p, JSON.stringify(state, null, 2), 'utf8');
  });

  ipcMain.handle('export-json-dialog', async (_, defaultName, payload) => {
    const r = await dialog.showSaveDialog({
      title: 'Export JSON',
      defaultPath: defaultName,
      filters: [{ name: 'JSON', extensions: ['json'] }]
    });
    if (r.canceled || !r.filePath) return null;
    fs.writeFileSync(r.filePath, JSON.stringify(payload, null, 2), 'utf8');
    return r.filePath;
  });

  ipcMain.handle('import-json-dialog', async () => {
    const r = await dialog.showOpenDialog({ filters: [{ name: 'JSON', extensions: ['json'] }], properties: ['openFile'] });
    if (r.canceled || !r.filePaths.length) return null;
    const filePath = r.filePaths[0];
    return { filePath, content: fs.readFileSync(filePath, 'utf8') };
  });

  ipcMain.handle('translate-text', async (_, payload) => translateText(payload));
  ipcMain.handle('set-dirty', (event, dirty) => {
    const id = event.sender.id;
    if (dirty) dirtyWindows.add(id);
    else dirtyWindows.delete(id);
    return true;
  });

  createWindow();
  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
