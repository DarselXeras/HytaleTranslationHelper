const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
  chooseRootFolder: () => ipcRenderer.invoke('choose-root-folder'),
  getAvailableLangFiles: (root) => ipcRenderer.invoke('get-available-lang-files', root),
  loadAll: (root, fileName) => ipcRenderer.invoke('load-all', root, fileName),
  saveAll: (root, fileName, languages, data) => ipcRenderer.invoke('save-all', root, fileName, languages, data),
  createLanguage: (root, newLanguage, defaultLanguage) => ipcRenderer.invoke('create-language', root, newLanguage, defaultLanguage),
  loadState: () => ipcRenderer.invoke('load-state'),
  saveState: (state) => ipcRenderer.invoke('save-state', state),
  exportJsonDialog: (defaultName, payload) => ipcRenderer.invoke('export-json-dialog', defaultName, payload),
  importJsonDialog: () => ipcRenderer.invoke('import-json-dialog'),
  translateText: (payload) => ipcRenderer.invoke('translate-text', payload),
  setDirty: (dirty) => ipcRenderer.invoke('set-dirty', dirty)
});
