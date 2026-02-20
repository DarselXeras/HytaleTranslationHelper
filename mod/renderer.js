const I18N = {
  de: {
    ready: 'Bereit.', chooseFolder: 'Ordner waehlen', refresh: 'Aktualisieren', save: 'Speichern',
    importJson: 'JSON Import', exportJson: 'JSON Export', createLanguage: 'Neue Sprache', settings: 'Uebersetzungs-URL',
    searchPh: 'Key suchen...', onlyMissing: 'Nur fehlende', filter: 'Filter', nextMissing: 'Naechstes fehlendes',
    autoTranslate: 'Auto-Uebersetzung', copyKey: 'Key kopieren', key: 'Key',
    noFile: 'Keine .lang-Datei gefunden.', loadFirst: 'Bitte zuerst laden.', selectKey: 'Bitte erst einen Key auswaehlen.', unsavedRefreshConfirm: 'Es gibt ungespeicherte Aenderungen. Wirklich aktualisieren und Aenderungen verwerfen?',
    saved: 'Gespeichert.', createdLanguage: 'Neue Sprache angelegt:',
    nextMissingNone: 'Keine fehlenden Uebersetzungen im Filter.',
    statusSummary: 'Sprachen: {0} | Keys: {1} | Fehlende: {2}',
    autoTranslated: 'Auto-uebersetzt: {0}', copiedKey: 'Key kopiert: {0}', autoTranslateFailed: 'Auto-Uebersetzen fehlgeschlagen:',
    modalCreateLanguage: 'Neue Sprache anlegen', modalAddEntry: 'Eintrag hinzufuegen', modalImportMode: 'JSON Import', modalTranslateUrl: 'Uebersetzungs-URL',
    newLanguage: 'Neue Sprache (Ordnername)', fallbackLanguage: 'Fallback-Sprache (optional)', noneOption: '(Keine - nur Keys uebernehmen)',
    importModeLabel: 'Import-Modus', importModeMerge: 'Zusammenfuehren', importModeReplace: 'Ersetzen',
    translateUrlLabel: 'LibreTranslate URL (optional)', ok: 'OK', cancel: 'Abbrechen',
    invalidJson: 'Ungueltige JSON-Datei.', noSourceText: 'Keine Quelluebersetzung vorhanden.',
    addEntryPath: 'Pfad', addEntrySuffix: 'Bezeichnung', addEntryDefault: '',
    suffixNoDot: 'Bitte nur den letzten Key-Teil eingeben (ohne Punkt).', keyExists: 'Key existiert bereits.',
    cannotAddUnderTranslatedKey: 'Unter diesem Pfad kann kein Unter-Key angelegt werden, weil der Key selbst bereits Uebersetzungen enthaelt.'
  },
  en: {
    ready: 'Ready.', chooseFolder: 'Choose folder', refresh: 'Refresh', save: 'Save',
    importJson: 'Import JSON', exportJson: 'Export JSON', createLanguage: 'Create language', settings: 'Translation URL',
    searchPh: 'Search key...', onlyMissing: 'Only missing', filter: 'Filter', nextMissing: 'Next missing',
    autoTranslate: 'Auto-translation', copyKey: 'Copy key', key: 'Key',
    noFile: 'No .lang file found.', loadFirst: 'Please load first.', selectKey: 'Please select a key first.', unsavedRefreshConfirm: 'There are unsaved changes. Refresh anyway and discard changes?',
    saved: 'Saved.', createdLanguage: 'Created language:', nextMissingNone: 'No missing translations in filter.',
    statusSummary: 'Languages: {0} | Keys: {1} | Missing: {2}', autoTranslated: 'Auto-translated: {0}', copiedKey: 'Key copied: {0}', autoTranslateFailed: 'Auto-translation failed:',
    modalCreateLanguage: 'Create new language', modalAddEntry: 'Add entry', modalImportMode: 'JSON Import', modalTranslateUrl: 'Translation URL',
    newLanguage: 'New language (folder name)', fallbackLanguage: 'Fallback language (optional)', noneOption: '(None - create keys only)',
    importModeLabel: 'Import mode', importModeMerge: 'Merge', importModeReplace: 'Replace',
    translateUrlLabel: 'LibreTranslate URL (optional)', ok: 'OK', cancel: 'Cancel',
    invalidJson: 'Invalid JSON file.', noSourceText: 'No source translation available.',
    addEntryPath: 'Path', addEntrySuffix: 'Label', addEntryDefault: '',
    suffixNoDot: 'Please enter only the last key segment (without dot).', keyExists: 'Key already exists.',
    cannotAddUnderTranslatedKey: 'You cannot add a sub-key below this path because the key itself already has translations.'
  }
};

let state = { rootPath: '', selectedLangFileName: '', uiLanguage: 'de', translateUrl: 'https://libretranslate.com/translate' };
let languages = [];
let langData = {};
let selectedKey = null;
let hasUnsaved = false;
const treeState = new Set();

const el = {
  btnChooseFolder: document.getElementById('btnChooseFolder'), selFile: document.getElementById('selFile'),
  btnRefresh: document.getElementById('btnRefresh'), btnSave: document.getElementById('btnSave'),
  btnImportJson: document.getElementById('btnImportJson'), btnExportJson: document.getElementById('btnExportJson'),
  btnCreateLanguage: document.getElementById('btnCreateLanguage'), btnSettings: document.getElementById('btnSettings'),
  selUiLang: document.getElementById('selUiLang'), txtSearch: document.getElementById('txtSearch'),
  chkMissing: document.getElementById('chkMissing'), btnFilter: document.getElementById('btnFilter'),
  btnNextMissing: document.getElementById('btnNextMissing'), btnAutoTranslate: document.getElementById('btnAutoTranslate'), btnCopyKey: document.getElementById('btnCopyKey'),
  tree: document.getElementById('tree'), selectedKey: document.getElementById('selectedKey'), editors: document.getElementById('editors'), status: document.getElementById('status'),
  modalBackdrop: document.getElementById('modalBackdrop'), modalTitle: document.getElementById('modalTitle'), modalBody: document.getElementById('modalBody'),
  modalOk: document.getElementById('modalOk'), modalCancel: document.getElementById('modalCancel')
};

const t = (k) => I18N[state.uiLanguage]?.[k] ?? k;
const tf = (k, ...args) => args.reduce((acc, v, i) => acc.replace(`{${i}}`, v), t(k));
const langCode = (lang) => (lang?.split('-')[0] || 'en').toLowerCase();

function setStatus(text) { el.status.textContent = text; }
function setDirty(flag) {
  hasUnsaved = !!flag;
  window.api.setDirty(hasUnsaved);
}

function updateStaticTexts() {
  el.btnChooseFolder.textContent = t('chooseFolder');
  el.btnRefresh.textContent = t('refresh');
  el.btnSave.textContent = t('save');
  el.btnImportJson.textContent = t('importJson');
  el.btnExportJson.textContent = t('exportJson');
  el.btnCreateLanguage.textContent = t('createLanguage');
  el.btnSettings.textContent = t('settings');
  el.txtSearch.placeholder = t('searchPh');
  el.chkMissing.parentElement.lastChild.textContent = ` ${t('onlyMissing')}`;
  el.btnFilter.textContent = t('filter');
  el.btnNextMissing.textContent = t('nextMissing');
  el.btnAutoTranslate.textContent = t('autoTranslate');
  el.btnCopyKey.textContent = t('copyKey');
  el.modalOk.textContent = t('ok');
  el.modalCancel.textContent = t('cancel');
}

function allKeys() {
  const set = new Set();
  for (const l of languages) for (const k of Object.keys(langData[l] || {})) set.add(k);
  return [...set].sort((a, b) => a.localeCompare(b));
}

function isMissingKey(key) {
  return languages.some(l => !langData[l]?.[key] || !String(langData[l][key]).trim());
}

function hasAnyTranslationForKey(key) {
  return languages.some(l => String(langData[l]?.[key] ?? '').trim().length > 0);
}

function filteredKeys() {
  const term = el.txtSearch.value.trim().toLowerCase();
  return allKeys().filter(k => (!term || k.toLowerCase().includes(term)) && (!el.chkMissing.checked || isMissingKey(k)));
}

function buildKeyTree(keys) {
  const root = {};
  for (const key of keys) {
    const parts = key.split('.');
    let cur = root;
    for (let i = 0; i < parts.length; i++) {
      const p = parts[i];
      cur[p] ??= { __children: {}, __key: null };
      if (i === parts.length - 1) cur[p].__key = key;
      cur = cur[p].__children;
    }
  }
  return root;
}

function renderTreeNode(container, nodeObj, path = '') {
  const ul = document.createElement('ul');
  for (const part of Object.keys(nodeObj).sort((a, b) => a.localeCompare(b))) {
    const node = nodeObj[part];
    const fullPath = path ? `${path}.${part}` : part;
    const hasChildren = Object.keys(node.__children).length > 0;
    const isLeaf = !!node.__key;

    const li = document.createElement('li');
    const row = document.createElement('div');
    row.className = 'tree-row' + (node.__key === selectedKey ? ' active' : '');
    row.dataset.path = fullPath;
    row.dataset.isLeaf = isLeaf ? '1' : '0';
    row.dataset.key = node.__key || '';

    const toggle = document.createElement('span');
    toggle.className = 'tree-toggle';
    toggle.textContent = hasChildren ? (treeState.has(fullPath) ? 'v' : '>') : '';

    const label = document.createElement('span');
    label.className = 'tree-label ' + (hasChildren ? 'folder' : 'leaf');
    label.textContent = part;

    row.append(toggle, label);

    let childWrap = null;
    if (hasChildren) {
      childWrap = document.createElement('div');
      if (!treeState.has(fullPath)) childWrap.classList.add('hidden');
      renderTreeNode(childWrap, node.__children, fullPath);

      toggle.onclick = (e) => {
        e.stopPropagation();
        if (treeState.has(fullPath)) treeState.delete(fullPath); else treeState.add(fullPath);
        buildTree();
      };
    }

    row.onclick = () => {
      if (isLeaf) selectedKey = node.__key;
      else if (hasChildren) {
        if (treeState.has(fullPath)) treeState.delete(fullPath); else treeState.add(fullPath);
      }
      buildTree();
      buildEditors();
    };

    li.appendChild(row);
    if (childWrap) li.appendChild(childWrap);
    ul.appendChild(li);
  }
  container.appendChild(ul);
}

function buildTree() {
  el.tree.innerHTML = '';
  const tree = buildKeyTree(filteredKeys());
  renderTreeNode(el.tree, tree);
}

function buildEditors() {
  el.selectedKey.textContent = `${t('key')}: ${selectedKey ?? '-'}`;
  el.editors.innerHTML = '';
  if (!selectedKey) return;

  for (const l of languages) {
    const row = document.createElement('div'); row.className = 'editor-row';
    const lbl = document.createElement('label'); lbl.textContent = l;
    const ta = document.createElement('textarea'); ta.value = langData[l]?.[selectedKey] ?? '';
    ta.oninput = () => {
      langData[l] ??= {};
      langData[l][selectedKey] = ta.value;
      setDirty(true);
      updateStatusSummary();
    };
    row.append(lbl, ta);
    el.editors.appendChild(row);
  }
}

function updateStatusSummary() {
  if (!languages.length) return setStatus(t('ready'));
  const keys = allKeys();
  let missing = 0;
  for (const k of keys) for (const l of languages) if (!langData[l]?.[k]?.trim()) missing++;
  setStatus(tf('statusSummary', languages.length, keys.length, missing));
}

function showModal({ title, bodyBuilder, onOk }) {
  return new Promise((resolve) => {
    el.modalTitle.textContent = title;
    el.modalBody.innerHTML = '';
    const ctx = bodyBuilder(el.modalBody);

    const close = (result) => {
      el.modalBackdrop.classList.add('hidden');
      el.modalOk.onclick = null;
      el.modalCancel.onclick = null;
      resolve(result);
    };

    el.modalOk.onclick = async () => {
      const r = await onOk(ctx);
      if (r !== undefined) close(r);
    };
    el.modalCancel.onclick = () => close(null);
    el.modalBackdrop.classList.remove('hidden');
  });
}

async function reload() {
  if (!state.rootPath || !state.selectedLangFileName) return;
  const payload = await window.api.loadAll(state.rootPath, state.selectedLangFileName);
  languages = payload.languages;
  langData = payload.data;
  if (!allKeys().includes(selectedKey)) selectedKey = allKeys()[0] ?? null;
  buildTree(); buildEditors(); updateStatusSummary();
}

async function refreshFilesAndMaybeLoad() {
  if (!state.rootPath) return;
  const files = await window.api.getAvailableLangFiles(state.rootPath);
  el.selFile.innerHTML = '';
  for (const f of files) {
    const o = document.createElement('option'); o.value = f; o.textContent = f; el.selFile.appendChild(o);
  }
  if (!files.length) return setStatus(t('noFile'));

  if (!state.selectedLangFileName || !files.includes(state.selectedLangFileName)) state.selectedLangFileName = files[0];
  el.selFile.value = state.selectedLangFileName;
  await window.api.saveState(state);
  await reload();
}

async function addEntryAtPathFlow(pathPrefix) {
  if (!pathPrefix) return;
  if (allKeys().includes(pathPrefix) && hasAnyTranslationForKey(pathPrefix)) return alert(t('cannotAddUnderTranslatedKey'));

  const suffix = await showModal({
    title: t('modalAddEntry'),
    bodyBuilder: (root) => {
      const f1 = document.createElement('div'); f1.className = 'field';
      const l1 = document.createElement('label'); l1.textContent = t('addEntryPath');
      const i1 = document.createElement('input'); i1.value = pathPrefix; i1.readOnly = true; f1.append(l1, i1);

      const f2 = document.createElement('div'); f2.className = 'field';
      const l2 = document.createElement('label'); l2.textContent = t('addEntrySuffix');
      const i2 = document.createElement('input'); i2.value = t('addEntryDefault'); f2.append(l2, i2);
      root.append(f1, f2); i2.focus(); i2.select();
      return { i2 };
    },
    onOk: ({ i2 }) => {
      const s = i2.value.trim();
      if (!s) return undefined;
      if (s.includes('.')) return alert(t('suffixNoDot')), undefined;
      return s;
    }
  });

  if (!suffix) return;
  const newKey = `${pathPrefix}.${suffix}`;
  if (allKeys().includes(newKey)) return alert(t('keyExists'));

  for (const l of languages) {
    langData[l] ??= {};
    langData[l][newKey] = '';
  }
  selectedKey = newKey;
  setDirty(true);
  buildTree(); buildEditors(); updateStatusSummary();
}

async function createLanguageFlow() {
  if (!state.rootPath) return alert(t('loadFirst'));

  const result = await showModal({
    title: t('modalCreateLanguage'),
    bodyBuilder: (root) => {
      const f1 = document.createElement('div'); f1.className = 'field';
      const l1 = document.createElement('label'); l1.textContent = t('newLanguage');
      const input = document.createElement('input'); input.value = 'fr-FR'; f1.append(l1, input);

      const f2 = document.createElement('div'); f2.className = 'field';
      const l2 = document.createElement('label'); l2.textContent = t('fallbackLanguage');
      const sel = document.createElement('select');
      const none = document.createElement('option'); none.value = ''; none.textContent = t('noneOption'); sel.appendChild(none);
      for (const l of languages) { const o = document.createElement('option'); o.value = l; o.textContent = l; sel.appendChild(o); }
      f2.append(l2, sel);

      root.append(f1, f2); input.focus();
      return { input, sel };
    },
    onOk: ({ input, sel }) => {
      const n = input.value.trim();
      if (!n) return undefined;
      return { newLanguage: n, defaultLanguage: sel.value || null };
    }
  });

  if (!result) return;
  try {
    await window.api.createLanguage(state.rootPath, result.newLanguage, result.defaultLanguage);
    await refreshFilesAndMaybeLoad();
    setStatus(`${t('createdLanguage')} ${result.newLanguage}`);
  } catch (e) { alert(e.message || String(e)); }
}

async function importJsonFlow() {
  const got = await window.api.importJsonDialog();
  if (!got) return;

  let importObj;
  try { importObj = JSON.parse(got.content); } catch { return alert(t('invalidJson')); }
  if (!Array.isArray(importObj.languages) || !importObj.entries) return alert(t('invalidJson'));

  const mode = await showModal({
    title: t('modalImportMode'),
    bodyBuilder: (root) => {
      const f = document.createElement('div'); f.className = 'field';
      const l = document.createElement('label'); l.textContent = t('importModeLabel');
      const s = document.createElement('select');
      const m = document.createElement('option'); m.value = 'merge'; m.textContent = t('importModeMerge');
      const r = document.createElement('option'); r.value = 'replace'; r.textContent = t('importModeReplace');
      s.append(m, r); f.append(l, s); root.append(f);
      return { s };
    },
    onOk: ({ s }) => s.value
  });

  if (!mode) return;

  if (mode === 'replace') {
    languages = [...new Set(importObj.languages)].sort((a, b) => a.localeCompare(b));
    langData = Object.fromEntries(languages.map(l => [l, {}]));
  } else {
    for (const l of importObj.languages) if (!languages.includes(l)) { languages.push(l); langData[l] = {}; }
    languages.sort((a, b) => a.localeCompare(b));
  }

  for (const [key, values] of Object.entries(importObj.entries)) {
    for (const l of languages) {
      langData[l] ??= {};
      langData[l][key] = values?.[l] ?? langData[l][key] ?? '';
    }
  }

  if (importObj.fileName) {
    state.selectedLangFileName = importObj.fileName;
    await window.api.saveState(state);
  }

  setDirty(true);
  buildTree(); buildEditors(); updateStatusSummary();
}

async function exportJsonFlow() {
  if (!languages.length) return alert(t('loadFirst'));
  const entries = {};
  for (const key of allKeys()) {
    entries[key] = {};
    for (const l of languages) entries[key][l] = langData[l]?.[key] ?? '';
  }
  const payload = { fileName: state.selectedLangFileName || '', languages, entries };
  const saved = await window.api.exportJsonDialog((state.selectedLangFileName || 'languages') + '.json', payload);
  if (saved) setStatus(`JSON exported: ${saved}`);
}

async function translationSettingsFlow() {
  const url = await showModal({
    title: t('modalTranslateUrl'),
    bodyBuilder: (root) => {
      const f = document.createElement('div'); f.className = 'field';
      const l = document.createElement('label'); l.textContent = t('translateUrlLabel');
      const i = document.createElement('input'); i.value = state.translateUrl || '';
      f.append(l, i); root.append(f);
      return { i };
    },
    onOk: ({ i }) => i.value.trim()
  });
  if (url === null) return;
  state.translateUrl = url || 'https://libretranslate.com/translate';
  await window.api.saveState(state);
}

async function autoTranslateCurrentKey() {
  if (!selectedKey) return alert(t('selectKey'));
  if (languages.length < 2) return alert(t('loadFirst'));

  const sourceLang = languages.find(l => (langData[l]?.[selectedKey] || '').trim());
  if (!sourceLang) return alert(t('noSourceText'));

  const sourceText = langData[sourceLang][selectedKey].trim();
  let changed = 0;

  el.btnAutoTranslate.disabled = true;
  try {
    for (const targetLang of languages) {
      if (targetLang === sourceLang) continue;
      if ((langData[targetLang]?.[selectedKey] || '').trim()) continue;
      const res = await window.api.translateText({ text: sourceText, source: langCode(sourceLang), target: langCode(targetLang), baseUrl: state.translateUrl });
      if (res?.translatedText) {
        if (res.usedEndpoint && res.usedEndpoint !== 'mymemory') state.translateUrl = res.usedEndpoint;
        langData[targetLang] ??= {};
        langData[targetLang][selectedKey] = res.translatedText;
        changed++;
      }
    }
    await window.api.saveState(state);
    if (changed > 0) {
      setDirty(true);
      buildEditors();
      updateStatusSummary();
      setStatus(tf('autoTranslated', changed));
    }
  } catch (e) {
    alert(`${t('autoTranslateFailed')}\n${e.message || e}`);
  } finally {
    el.btnAutoTranslate.disabled = false;
  }
}

async function copySelectedKey() {
  if (!selectedKey) return alert(t('selectKey'));
  try { await navigator.clipboard.writeText(selectedKey); }
  catch {
    const ta = document.createElement('textarea');
    ta.value = selectedKey; document.body.appendChild(ta); ta.select(); document.execCommand('copy'); ta.remove();
  }
  setStatus(tf('copiedKey', selectedKey));
}

function wire() {
  el.btnChooseFolder.onclick = async () => {
    const p = await window.api.chooseRootFolder();
    if (!p) return;
    state.rootPath = p;
    await window.api.saveState(state);
    await refreshFilesAndMaybeLoad();
  };

  el.selFile.onchange = async () => {
    state.selectedLangFileName = el.selFile.value;
    await window.api.saveState(state);
    await reload();
  };

  el.btnRefresh.onclick = async () => {
    if (hasUnsaved) {
      const ok = confirm(t('unsavedRefreshConfirm'));
      if (!ok) return;
    }
    await refreshFilesAndMaybeLoad();
  };
  el.btnFilter.onclick = () => {
    buildTree();
    if (!filteredKeys().includes(selectedKey)) selectedKey = filteredKeys()[0] || null;
    buildEditors();
    updateStatusSummary();
  };

  el.btnNextMissing.onclick = () => {
    const keys = filteredKeys();
    const idx = Math.max(-1, keys.indexOf(selectedKey));
    for (let i = 1; i <= keys.length; i++) {
      const k = keys[(idx + i) % keys.length];
      if (isMissingKey(k)) {
        selectedKey = k;
        buildTree();
        buildEditors();
        setStatus(`${t('nextMissing')}: ${k}`);
        return;
      }
    }
    setStatus(t('nextMissingNone'));
  };

  el.btnSave.onclick = async () => {
    if (!state.rootPath || !state.selectedLangFileName) return alert(t('loadFirst'));
    await window.api.saveAll(state.rootPath, state.selectedLangFileName, languages, langData);
    setDirty(false);
    setStatus(t('saved'));
  };

  el.btnCreateLanguage.onclick = createLanguageFlow;
  el.btnImportJson.onclick = importJsonFlow;
  el.btnExportJson.onclick = exportJsonFlow;
  el.btnAutoTranslate.onclick = autoTranslateCurrentKey;
  el.btnCopyKey.onclick = copySelectedKey;
  el.btnSettings.onclick = translationSettingsFlow;

  // RIGHT CLICK in tree: add entry at path directly
  el.tree.addEventListener('contextmenu', async (e) => {
    const row = e.target.closest('.tree-row');
    if (!row) return;
    e.preventDefault();

    const fullPath = row.dataset.path || '';
    const isLeaf = row.dataset.isLeaf === '1';
    const key = row.dataset.key || null;

    if (isLeaf && key) selectedKey = key;

    const addPath = isLeaf
      ? (fullPath.includes('.') ? fullPath.slice(0, fullPath.lastIndexOf('.')) : '')
      : fullPath;

    if (!addPath) return;

    buildTree();
    buildEditors();
    await addEntryAtPathFlow(addPath);
  });

  el.selUiLang.onchange = async () => {
    state.uiLanguage = el.selUiLang.value;
    await window.api.saveState(state);
    updateStaticTexts();
    buildTree();
    buildEditors();
    updateStatusSummary();
  };

}

(async function init() {
  const loaded = await window.api.loadState();
  state = { ...state, ...loaded };
  el.selUiLang.value = state.uiLanguage;
  updateStaticTexts();
  wire();
  setDirty(false);
  if (state.rootPath) await refreshFilesAndMaybeLoad();
  else setStatus(t('ready'));
})();

