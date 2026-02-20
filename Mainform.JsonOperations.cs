using System.Text;
using System.Text.Json;

namespace LanguageFileEditor;

public partial class Mainform
{
    private void ExportJson()
    {
        try
        {
            if (_languages.Count == 0)
                throw new InvalidOperationException(S("msg.loadFirst"));

            using var dlg = new SaveFileDialog
            {
                Filter = S("json.dialog.filter"),
                FileName = (_selectedLangFileName ?? "languages") + ".json",
                Title = S("json.export.title")
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var keys = _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal).OrderBy(k => k).ToList();
            var entries = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

            foreach (var key in keys)
            {
                var perLang = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var lang in _languages)
                    perLang[lang] = _langData[lang].GetValueOrDefault(key, string.Empty);

                entries[key] = perLang;
            }

            var export = new JsonLanguageExport
            {
                FileName = _selectedLangFileName ?? string.Empty,
                Languages = [.. _languages],
                Entries = entries
            };

            var json = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dlg.FileName, json, new UTF8Encoding(false));
            UpdateStatus(SF("status.jsonExported", Path.GetFileName(dlg.FileName)));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, S("err.jsonExportFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ImportJson()
    {
        try
        {
            using var dlg = new OpenFileDialog
            {
                Filter = S("json.dialog.filter"),
                Title = S("json.import.title")
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var json = File.ReadAllText(dlg.FileName, Encoding.UTF8);
            var import = JsonSerializer.Deserialize<JsonLanguageExport>(json)
                         ?? throw new InvalidOperationException(S("err.invalidJson"));

            var incomingLangs = import.Languages?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? [];

            if (incomingLangs.Count == 0)
                throw new InvalidOperationException(S("err.jsonNoLanguages"));

            if (import.Entries is null || import.Entries.Count == 0)
                throw new InvalidOperationException(S("err.jsonNoEntries"));

            var preview = BuildImportPreview(import, incomingLangs);
            if (!preview.CanImport)
            {
                MessageBox.Show(this, preview.Message, S("json.validation.title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var proceed = MessageBox.Show(
                this,
                preview.Message + Environment.NewLine + Environment.NewLine + S("json.import.continueQuestion"),
                S("json.preview.title"),
                MessageBoxButtons.YesNo,
                preview.WarningCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            if (proceed != DialogResult.Yes) return;

            var replace = MessageBox.Show(
                this,
                S("json.import.replaceQuestion"),
                S("json.import.title"),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (replace == DialogResult.Cancel) return;

            if (replace == DialogResult.Yes)
            {
                _languages = incomingLangs.OrderBy(x => x).ToList();
                _langData.Clear();
                foreach (var lang in _languages)
                    _langData[lang] = new Dictionary<string, string>(StringComparer.Ordinal);
            }
            else
            {
                foreach (var lang in incomingLangs)
                {
                    if (_languages.Any(x => x.Equals(lang, StringComparison.OrdinalIgnoreCase))) continue;
                    _languages.Add(lang);
                    _langData[lang] = new Dictionary<string, string>(StringComparer.Ordinal);
                }

                _languages = _languages.OrderBy(x => x).ToList();
            }

            foreach (var (key, values) in preview.ValidEntries)
            {
                foreach (var lang in _languages)
                {
                    if (!_langData.TryGetValue(lang, out var dict))
                    {
                        dict = new Dictionary<string, string>(StringComparer.Ordinal);
                        _langData[lang] = dict;
                    }

                    dict[key] = values.TryGetValue(lang, out var value)
                        ? value ?? string.Empty
                        : dict.GetValueOrDefault(key, string.Empty);
                }
            }

            if (!string.IsNullOrWhiteSpace(import.FileName))
            {
                _selectedLangFileName = import.FileName;
                SaveStartupPath();
            }

            BuildEditors();
            ApplyFilters();
            _hasUnsavedChanges = true;
            UpdateStatus(SF("status.jsonImported", Path.GetFileName(dlg.FileName)));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, S("err.jsonImportFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private ImportPreview BuildImportPreview(JsonLanguageExport import, List<string> incomingLangs)
    {
        var validEntries = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        var unknownLangs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var emptyKeyCount = 0;
        var duplicateKeyCount = 0;
        var nullValueMaps = 0;
        var missingValueCount = 0;

        foreach (var (rawKey, map) in import.Entries)
        {
            var key = rawKey?.Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                emptyKeyCount++;
                continue;
            }

            if (validEntries.ContainsKey(key))
            {
                duplicateKeyCount++;
                continue;
            }

            var sourceValues = map;
            if (sourceValues is null)
            {
                nullValueMaps++;
                sourceValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var normalized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var lang in incomingLangs)
            {
                if (sourceValues.TryGetValue(lang, out var value))
                    normalized[lang] = value ?? string.Empty;
                else
                {
                    normalized[lang] = string.Empty;
                    missingValueCount++;
                }
            }

            foreach (var lang in sourceValues.Keys)
            {
                if (!incomingLangs.Contains(lang, StringComparer.OrdinalIgnoreCase))
                    unknownLangs.Add(lang);
            }

            validEntries[key] = normalized;
        }

        var lines = new List<string>
        {
            SF("json.preview.file", import.FileName),
            SF("json.preview.languages", incomingLangs.Count, string.Join(", ", incomingLangs)),
            SF("json.preview.totalEntries", import.Entries.Count),
            SF("json.preview.importableEntries", validEntries.Count)
        };

        var warnings = new List<string>();
        if (emptyKeyCount > 0) warnings.Add(SF("json.warn.emptyKeys", emptyKeyCount));
        if (duplicateKeyCount > 0) warnings.Add(SF("json.warn.duplicateKeys", duplicateKeyCount));
        if (nullValueMaps > 0) warnings.Add(SF("json.warn.nullMaps", nullValueMaps));
        if (missingValueCount > 0) warnings.Add(SF("json.warn.missingValues", missingValueCount));
        if (unknownLangs.Count > 0)
        {
            var sample = string.Join(", ", unknownLangs.Take(8));
            warnings.Add(SF("json.warn.unknownLangs", sample + (unknownLangs.Count > 8 ? " ..." : string.Empty)));
        }

        if (warnings.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add(S("json.preview.validationNotes"));
            lines.AddRange(warnings.Select(w => "- " + w));
        }

        if (validEntries.Count == 0)
        {
            lines.Add(string.Empty);
            lines.Add(S("json.preview.noValidEntries"));
        }

        return new ImportPreview
        {
            CanImport = validEntries.Count > 0,
            WarningCount = warnings.Count,
            Message = string.Join(Environment.NewLine, lines),
            ValidEntries = validEntries
        };
    }
}

