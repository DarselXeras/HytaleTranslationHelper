# Hytale Languagefile Editor (Electron) — Detailed User Manual (EN)

## 1) Overview
**Hytale Languagefile Editor** is an external desktop tool for managing Hytale `.lang` files across multiple languages.

Typical structure:
- `Languages/de-DE/<file>.lang`
- `Languages/en-US/<file>.lang`
- `Languages/fr-FR/<file>.lang`

Main capabilities:
- Side-by-side translation editing
- Missing-translation filtering/navigation
- JSON import/export
- Create new language folders (optional fallback)
- Add keys directly from tree path (right click)
- Auto-translation for empty target fields

---

## 2) System Requirements
- **OS:** Windows 10/11 (64-bit)
- **CPU:** x64 processor
- **RAM:** 4 GB minimum (8 GB recommended)
- **Disk space:** ~500 MB free (app + temporary build/runtime files)
- **Network:** Required for auto-translation endpoints (LibreTranslate/MyMemory)
- **Permissions:** Read/write access to your mod language folders

Notes:
- Auto-translation needs internet access.
- Unsigned EXE builds can trigger occasional antivirus false positives.

---

## 3) Delivered Build (your current release)
You are shipping this unpacked build:

`I:\Projekte\HytaleTranslationHelper\mod\dist\win-unpacked\Hytale Languagefile Editor`

Start the app via:

`Hytale Languagefile Editor.exe`

If needed, create a desktop shortcut to this EXE.

---

## 4) Installation Options
### Option A: Portable / unpacked (current)
1. Copy the full folder:
   - `mod\dist\win-unpacked\Hytale Languagefile Editor`
2. Run `Hytale Languagefile Editor.exe`.

### Option B: Installer build (optional)
From `mod/` run:

```powershell
.\build-setup.ps1
```

Then use the generated setup in `mod\dist\`.

---

## 5) Main UI Layout
### Top toolbar
- **Choose folder**: Select Languages root directory
- **File dropdown**: Select `.lang` file (e.g. `Items.lang`)
- **Refresh**
- **Save**
- **Import JSON / Export JSON**
- **Create language**
- **Translation URL**
- **UI language** (DE/EN)

### Filter bar
- Search
- Only missing
- Filter
- Next missing
- Auto-translation
- Copy key

### Main area
- **Left:** key tree
- **Right:** selected key + language editors

### Bottom
- Status line (languages, keys, missing count, operation feedback)

---

## 6) First Start Workflow
1. Click **Choose folder**.
2. Select your `Languages` root.
3. Select target `.lang` file from dropdown.
4. Click a key in tree.
5. Edit translations.
6. Click **Save**.

---

## 7) Editing and Key Management
### Edit values
- Modify text per language in right-side editors.
- Unsaved changes are tracked automatically.

### Copy selected key
- Click **Copy key** to copy full key path to clipboard.

### Add key from tree path (right click)
- Right-click a tree node.
- Enter **Label**.
- App creates `<path>.<label>` in all languages with empty values.

Safety rule:
- If clicked path itself is already a translated terminal key, creating sub-keys below it is blocked.

---

## 8) Search / Missing Navigation
- Search field filters keys by text.
- **Only missing** shows incomplete keys only.
- **Next missing** jumps to next incomplete key in current filter result.

---

## 9) Refresh Behavior
If unsaved changes exist and you click **Refresh**, app asks for confirmation before discarding changes.

---

## 10) Create New Language
Use **Create language**.

Fields:
- New language folder (e.g. `it-IT`)
- Optional fallback language

Behavior:
- Creates language folder
- Creates all existing `.lang` files
- With fallback: copy fallback values
- Without fallback: create keys with empty values

---

## 11) JSON Import / Export
### Export
- Exports current loaded file scope across all languages.

### Import
- Choose mode:
  - **Merge**
  - **Replace**

Expected format:
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

## 12) Auto-Translation
- Uses first non-empty language value as source.
- Fills missing target fields only.
- Tries configured LibreTranslate endpoint + fallback endpoints.
- Falls back to MyMemory if needed.

Configure endpoint via **Translation URL**.

---

## 13) Config and Persistence
State file is stored next to executable context:
- `config/lfe.config`

Stored:
- last root path
- selected `.lang` file
- UI language
- translation endpoint URL

---

## 14) Exit Behavior
If there are unsaved changes, closing the app prompts confirmation before exit.

---

## 15) Troubleshooting
### App won’t find `.lang` files
- Check folder structure and selected root.

### Auto-translation fails
- Check internet access and endpoint URL.
- Public endpoints can be rate-limited.

### Encoding glitches
- Ensure UTF-8 files.
- Restart app after updates.

### Push/build issues with large files
- Do not commit `mod/dist` and `mod/node_modules` to Git.
- Prefer GitHub Releases for installer uploads.

---

## 16) Release Recommendation
For end users, prefer distributing:
- installer EXE from `mod/dist/`
- or zipped portable folder `win-unpacked/Hytale Languagefile Editor`

Include checksum (SHA256) and a short AV false-positive note for unsigned builds.
