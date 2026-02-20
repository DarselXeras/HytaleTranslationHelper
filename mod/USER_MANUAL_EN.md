# Hytale Translation Helper (Electron) — Detailed User Manual (EN)

## 1) Overview
Hytale Translation Helper (Electron) is a desktop tool for managing Hytale `.lang` files across multiple languages in one interface.

It is designed for folder structures like:

- `Languages/de-DE/<file>.lang`
- `Languages/en-US/<file>.lang`
- `Languages/fr-FR/<file>.lang`

The app lets you:

- edit translations side-by-side,
- find missing entries quickly,
- import/export JSON,
- create new language folders with optional fallback,
- add keys directly from the tree path,
- auto-translate missing fields.

---

## 2) Installation

### Option A: Installer (recommended)
1. Run the setup EXE (generated in `mod/dist/`).
2. Follow installation steps.
3. Start **Hytale Translation Helper** from Start Menu / Desktop.

### Option B: Run from source
1. Install Node.js (LTS recommended).
2. Open terminal in `I:\Projekte\HytaleTranslationHelper\mod`.
3. Run:
   - `npm install`
   - `npm start`

---

## 3) Build your own setup
Use one of the included scripts in `mod/`:

- `build-setup.ps1`
- `build-setup.cmd`

PowerShell example:

```powershell
cd I:\Projekte\HytaleTranslationHelper\mod
.\build-setup.ps1
```

Output: `mod/dist/HytaleTranslationHelper-Setup-<version>.exe`

---

## 4) Main UI layout

### Top toolbar
- **Choose folder**: Select your Languages root directory.
- **File dropdown**: Select which `.lang` file to edit (e.g. `Items.lang`).
- **Refresh**: Reload file list + data.
- **Save**: Save all current changes.
- **Import JSON** / **Export JSON**
- **Create language**
- **Translation URL** (configure custom LibreTranslate endpoint)
- **UI language**: Deutsch / English

### Filter bar
- Search field
- **Only missing** toggle
- **Filter** button
- **Next missing** button
- **Auto-translation** button
- **Copy key** button

### Main area
- **Left**: hierarchical key tree
- **Right**: selected key + text editors per language

### Bottom
- Status line: language/key/missing counts and operation feedback.

---

## 5) First-time workflow
1. Click **Choose folder** and pick your `Languages` root.
2. Select a `.lang` file in the dropdown.
3. Pick a key in the tree.
4. Edit values in language text boxes.
5. Click **Save**.

---

## 6) Editing keys and translations

## Select a key
- Click any leaf in the tree.
- The right panel updates with that key for all loaded languages.

## Edit values
- Type translations directly into each language text area.
- Changes are tracked as unsaved until you click **Save**.

## Copy selected key
- Click **Copy key** to copy the active key path (e.g. `Blocks.MyBlock.name`) to clipboard.

---

## 7) Add new key from tree path (right click)
You can add entries context-aware from the tree:

1. Right-click a node in the tree.
2. Enter **Label** in the dialog.
3. Confirm.

The app creates `<path>.<label>` in all languages with empty values.

### Safety rule (important)
If the clicked path is already a full key that contains translation text in at least one language, adding a sub-key below it is blocked.

Example:
- Existing translated key: `Blocks.DexPack_SPC.EmberTinkerbench.name`
- Creating `...name.anything` is prevented.

---

## 8) Search, missing filter, and navigation
- Use the search box to filter keys by text.
- Enable **Only missing** to see only incomplete keys.
- Click **Next missing** to jump to the next unresolved key in current filter scope.

---

## 9) Refresh behavior with unsaved changes
If there are unsaved edits and you click **Refresh**, the app asks for confirmation before discarding changes.

---

## 10) Create a new language
Click **Create language**.

Dialog fields:
- **New language (folder name)**: e.g. `it-IT`
- **Fallback language (optional)**

Behavior:
- Creates the language folder.
- Creates all existing `.lang` files for the new language.
- If fallback is selected: values are copied from fallback files.
- If no fallback: keys are created with empty values.

---

## 11) JSON export/import

## Export JSON
- Click **Export JSON**.
- Exports current `.lang` scope across all languages.

## Import JSON
- Click **Import JSON**.
- Choose mode:
  - **Merge**: keep existing data and add/update incoming entries.
  - **Replace**: replace current in-memory set with import content.

Expected JSON shape:

```json
{
  "fileName": "Items.lang",
  "languages": ["de-DE", "en-US"],
  "entries": {
    "Blocks.Example.name": {
      "de-DE": "Beispiel",
      "en-US": "Example"
    }
  }
}
```

---

## 12) Auto-translation
Click **Auto-translation** on a selected key.

Behavior:
- Uses the first non-empty language value as source.
- Fills missing target language fields only.
- Tries configured/default LibreTranslate endpoints.
- Falls back to MyMemory if needed.

### Configure endpoint
Use **Translation URL** to set a custom LibreTranslate URL.

Tip: self-hosted endpoint is recommended for privacy/control.

---

## 13) Persistence and config
The app stores state in:

- `config/lfe.config` (next to EXE)

Stored values include:
- last root folder
- selected `.lang` filename
- UI language
- translation endpoint URL

---

## 14) Exit behavior
If unsaved changes exist and you close the app, a confirmation dialog appears:
- Cancel close, or
- Exit without saving.

---

## 15) Troubleshooting

## “No .lang file found”
- Ensure selected root folder contains language subfolders with `.lang` files.

## Auto-translation fails
- Check internet connection.
- Check endpoint URL in **Translation URL**.
- Some public endpoints can be rate-limited/unavailable.

## Encoding/strange characters
- Ensure files are UTF-8.
- If issues appear after edits, rebuild/restart app.

## App not closing correctly
- Update to latest build; close handling is managed by main process now.

---

## 16) Recommended release process
1. `npm install`
2. `npm run dist`
3. Test setup from `mod/dist/`
4. Publish installer + optional SHA256 hash
5. Mention known false-positive possibility for unsigned executables

---

## 17) License / usage scope
This project is intended for private, non-commercial usage (as defined in repository license files).
