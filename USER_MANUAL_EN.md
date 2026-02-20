# User Manual – Hytale Translation Helper

## 1) Purpose
The **Hytale Translation Helper** is used to edit `.lang` files in a language-folder structure such as:

- `Languages/de-DE/items.lang`
- `Languages/en-US/items.lang`
- `Languages/fr-FR/items.lang`

The app shows all languages side-by-side for each key, helps you find missing translations, and supports JSON import/export.

---

## 2) Quick Start

1. Start the app.
2. Go to **File → Choose Languages folder...**
3. If multiple files exist, select one via **File → Choose language file...** (for example `items.lang`).
4. Select keys in the tree on the left and edit translations on the right.
5. Save using **File → Save**.

---

## 3) Interface Overview

### Left: Key Tree
- Shows hierarchical keys (split by `.`).
- Right-click context menu:
  - **Copy path to here**
  - **Add new entry here**

### Top: Search/Filter Bar
- **Search/Filter**: filter keys by text.
- **Only missing**: show only keys with missing translations.
- **Filter**: apply filter.
- **Next missing**: jump to the next incomplete key.
- **Auto-translation**: tries to fill missing fields for the currently selected key.

### Right: Language Editors
- One text box per language.
- Up to 5 languages are distributed evenly.
- More than 5 languages become scrollable.

### Bottom: Status Bar
- Shows summary information (languages, keys, missing translations).

---

## 4) Menu Functions

## File
- **Choose Languages folder...**
- **Choose language file...**
- **Create new language...**
- **Refresh list**
- **Save**
- **Import JSON...**
- **Export JSON...**
- **Exit**

## Edit
- **Add key**
- **Delete key**
- **Next missing field**
- **Copy path to here**

## Tools
- **Auto-translate (current key)**

## View
- **Toggle tree width**

## Language
- **Deutsch / English** (UI language)

---

## 5) Create New Language (important)
Use **File → Create new language...** to add a new language inside the currently selected Languages root.

### Dialog Input
- **New language (folder name)**, e.g. `it-IT`
- **Default/fallback language (optional)**
  - e.g. `en-US`
  - or **(None - create keys only)**

### Behavior
- The app creates the new language folder.
- It creates **all existing `.lang` files** found in other languages.
- If a fallback language is selected:
  - values are copied from that fallback file.
- If **None** is selected:
  - only keys are created,
  - values stay empty (`key = `), so you can translate everything manually.

---

## 6) JSON Import/Export

### Export
**File → Export JSON...**
- Exports the currently loaded language file across all languages into one JSON file.

### Import
**File → Import JSON...**
- Shows a validation/preview first.
- Then you can choose:
  - **Replace** (overwrite existing data)
  - **Merge** (combine with existing data)

---

## 7) Keyboard Shortcuts
- **F3**: next missing field
- **Ctrl + F**: focus search field
- **Delete** (in tree): delete selected key
- **Enter** (in search): apply filter

---

## 8) Storage & Config
The app stores its state locally next to the EXE:

- `config/lfe.config`

Includes:
- last selected Languages folder
- last selected `.lang` file
- UI language (DE/EN)

---

## 9) Auto-Translation Notice
Auto-translation uses external services (e.g. LibreTranslate endpoints, optional fallback).

- Do not send confidential content to public endpoints.
- For full control, use your own self-hosted translation endpoint.

---

## 10) Common Issues

### “No .lang file found/selected”
- Make sure `.lang` files exist in your language folders.
- Select one manually via **File → Choose language file...**.

### “Languages folder not found”
- Check the path and choose the folder again.

### Creating language fails
- Check for invalid characters in the new folder name.
- Check write permissions in the target directory.

---

## 11) License / Usage
This application is intended for **private, non-commercial use**.
