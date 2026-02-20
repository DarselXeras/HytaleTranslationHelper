using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LanguageFileEditor;

public partial class Form1 : Form
{
    // UI controls are declared/initialized in Form1.Designer.cs

    private readonly Dictionary<string, Dictionary<string, string>> _langData = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, TextBox> _langEditors = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(20) };
    private const string DefaultLanguagesPath = @"F:\Games\Hytale\UserData\Saves\Modding Test\mods\DarselX_EndlessEmber\Server\Languages";
    private static readonly string _stateFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "LanguageFileEditor",
        "state.json");
    private string _translateUrl = "https://libretranslate.com/translate";
    private string _rootPath = DefaultLanguagesPath;
    private string? _selectedLangFileName;
    private List<string> _languages = [];
    private string? _selectedKey;
    private bool _isBinding;
    private bool _hasUnsavedChanges;

    public Form1()
    {
        InitializeComponent();
        Width = 1450;
        Height = 900;

        WireEvents();
        ApplyFormIcon();

        LoadStartupPath();
        EnsureSelectedLangFile();
        UpdateStatus("Bereit.");
    }

    private void WireEvents()
    {
        // Toolbar actions
        _btnAddKey.Click += (_, _) => AddKey();
        _btnDeleteKey.Click += (_, _) => DeleteKey();
        _btnToggleTreeWidth.Click += (_, _) => ToggleTreeWidth();
        _btnAutoTranslate.Click += async (_, _) => await AutoTranslateSelectedKeyAsync();
        _btnApplyFilter.Click += (_, _) => ApplyFilters();
        _btnNextMissing.Click += (_, _) => SelectNextMissingKey();
        _btnExportJson.Click += (_, _) => ExportJson();
        _btnImportJson.Click += (_, _) => ImportJson();

        // Menu actions
        _mnuDateiOrdner.Click += (_, _) => BrowseFolder();
        _mnuDateiLaden.Click += (_, _) => RefreshListAndReload();
        _mnuDateiSpeichern.Click += (_, _) => SaveAll();
        _mnuDateiJsonImport.Click += (_, _) => ImportJson();
        _mnuDateiJsonExport.Click += (_, _) => ExportJson();
        _mnuDateiBeenden.Click += (_, _) => Close();

        _mnuBearbeitenAddKey.Click += (_, _) => AddKey();
        _mnuBearbeitenDeleteKey.Click += (_, _) => DeleteKey();
        _mnuBearbeitenNextMissing.Click += (_, _) => SelectNextMissingKey();
        _mnuBearbeitenCopyPath.Click += (_, _) => CopyPathAtSelectedNode();

        _mnuToolsAutoTranslate.Click += async (_, _) => await AutoTranslateSelectedKeyAsync();

        _mnuAnsichtToggleTreeWidth.Click += (_, _) => ToggleTreeWidth();

        _txtSearch.KeyDown += SearchKeyDown;
        _chkOnlyMissing.CheckedChanged += (_, _) => ApplyFilters();
        _tree.AfterSelect += (_, e) => { if (e.Node is not null) OnTreeSelectionChanged(e.Node); };
        _tree.KeyDown += TreeKeyDown;
        _tree.NodeMouseClick += TreeNodeMouseClick;
        _treeMenu.Opening += TreeMenuOpening;
        _miCopyPathAtNode.Click += (_, _) => CopyPathAtSelectedNode();
        _miAddEntryAtNode.Click += (_, _) => AddEntryAtSelectedNode();
        FormClosing += (_, _) => SaveStartupPath();
    }

    private void ToggleTreeWidth()
    {
        _middle.SplitterDistance = _middle.SplitterDistance <= 260 ? 360 : 220;
    }

    private void ApplyFormIcon()
    {
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "LanguageFileEditor_icon.ico");
            if (!File.Exists(iconPath)) return;
            Icon = new Icon(iconPath);
        }
        catch
        {
            // ignore icon load errors
        }
    }

    private void TreeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            e.Handled = true;
            DeleteKey();
            return;
        }

        if (e.KeyCode == Keys.F3)
        {
            e.Handled = true;
            SelectNextMissingKey();
            return;
        }

        if (e.Control && e.KeyCode == Keys.F)
        {
            e.Handled = true;
            _txtSearch.Focus();
            _txtSearch.SelectAll();
        }
    }

    private void SearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.Handled = true;
        e.SuppressKeyPress = true;
        ApplyFilters();
    }

    private void TreeNodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        _tree.SelectedNode = e.Node;
    }

    private void TreeMenuOpening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var hasSelection = _tree.SelectedNode is not null;
        _miCopyPathAtNode.Enabled = hasSelection;
        _miAddEntryAtNode.Enabled = hasSelection;
        e.Cancel = !hasSelection;
    }

    private void CopyPathAtSelectedNode()
    {
        var node = _tree.SelectedNode;
        if (node is null) return;

        var path = GetNodePath(node);
        if (string.IsNullOrWhiteSpace(path)) return;

        Clipboard.SetText(path);
        UpdateStatus($"Pfad kopiert: {path}");
    }

    private void AddEntryAtSelectedNode()
    {
        if (_languages.Count == 0)
        {
            MessageBox.Show(this, "Bitte zuerst eine Sprachdatei laden.");
            return;
        }

        var node = _tree.SelectedNode;
        if (node is null)
        {
            MessageBox.Show(this, "Bitte einen Knoten auswÃ¤hlen.");
            return;
        }

        // Hauptknoten = alles vor dem letzten Key-Segment
        // Rechtsklick auf Leaf -> Parent als Prefix verwenden
        var prefixNode = node.Tag is string ? node.Parent : node;
        if (prefixNode is null)
        {
            MessageBox.Show(this, "Bitte keinen Root-Leaf ohne Prefix wÃ¤hlen.");
            return;
        }

        var prefix = GetNodePath(prefixNode);
        if (string.IsNullOrWhiteSpace(prefix))
        {
            MessageBox.Show(this, "Knotenpfad konnte nicht ermittelt werden.");
            return;
        }

        var suffix = Microsoft.VisualBasic.Interaction.InputBox(
            $"Neuer Eintrag unter:\n{prefix}\n\nSuffix (letzter Teil nach dem Punkt):",
            "Eintrag hinzufÃ¼gen",
            "name");

        if (string.IsNullOrWhiteSpace(suffix)) return;
        suffix = suffix.Trim();

        if (suffix.Contains('.'))
        {
            MessageBox.Show(this, "Bitte nur den letzten Key-Teil eingeben (ohne Punkt).", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var newKey = $"{prefix}.{suffix}";
        var exists = _langData.Values.Any(d => d.ContainsKey(newKey));
        if (exists)
        {
            MessageBox.Show(this, $"Key existiert bereits:\n{newKey}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        foreach (var lang in _languages)
        {
            _langData[lang][newKey] = string.Empty;
        }

        BuildTree();
        SelectNodeByKey(newKey);
        _hasUnsavedChanges = true;
        UpdateStatus();
    }

    private static string GetNodePath(TreeNode node)
    {
        var parts = new Stack<string>();
        var current = node;
        while (current is not null)
        {
            parts.Push(current.Text);
            current = current.Parent;
        }
        return string.Join('.', parts);
    }

    private void LoadStartupPath()
    {
        try
        {
            var state = LoadState();
            var candidate = !string.IsNullOrWhiteSpace(state?.RootPath) ? state!.RootPath : DefaultLanguagesPath;
            _rootPath = candidate;
            _selectedLangFileName = string.IsNullOrWhiteSpace(state?.SelectedLangFileName) ? null : state!.SelectedLangFileName;
        }
        catch
        {
            _rootPath = DefaultLanguagesPath;
            _selectedLangFileName = null;
        }
    }

    private void SaveStartupPath()
    {
        try
        {
            var dir = Path.GetDirectoryName(_stateFilePath);
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);

            var state = new AppState
            {
                RootPath = _rootPath,
                SelectedLangFileName = _selectedLangFileName ?? string.Empty
            };
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_stateFilePath, json, Encoding.UTF8);
        }
        catch
        {
            // best effort
        }
    }

    private static AppState? LoadState()
    {
        if (!File.Exists(_stateFilePath)) return null;
        var json = File.ReadAllText(_stateFilePath, Encoding.UTF8);
        if (string.IsNullOrWhiteSpace(json)) return null;
        return JsonSerializer.Deserialize<AppState>(json);
    }

    private List<string> GetAvailableLangFiles(string root)
    {
        if (!Directory.Exists(root)) return [];

        return Directory.GetDirectories(root)
            .SelectMany(langDir => Directory.GetFiles(langDir, "*.lang").Select(Path.GetFileName))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
    }

    private bool EnsureSelectedLangFile(bool promptIfMultiple = false)
    {
        var files = GetAvailableLangFiles(_rootPath);
        if (files.Count == 0)
        {
            _selectedLangFileName = null;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(_selectedLangFileName) && files.Contains(_selectedLangFileName, StringComparer.OrdinalIgnoreCase))
            return true;

        if (files.Count == 1)
        {
            _selectedLangFileName = files[0];
            SaveStartupPath();
            return true;
        }

        if (!promptIfMultiple)
        {
            _selectedLangFileName = files[0];
            SaveStartupPath();
            return true;
        }

        var options = string.Join(Environment.NewLine, files.Select((f, i) => $"{i + 1}. {f}"));
        var input = Microsoft.VisualBasic.Interaction.InputBox(
            $"Mehrere .lang-Dateien gefunden. Bitte Namen eingeben:{Environment.NewLine}{Environment.NewLine}{options}",
            "Datei wählen",
            files[0]);

        if (string.IsNullOrWhiteSpace(input)) return false;
        var selected = files.FirstOrDefault(f => string.Equals(f, input.Trim(), StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            MessageBox.Show(this, "Datei nicht gefunden.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        _selectedLangFileName = selected;
        SaveStartupPath();
        return true;
    }

    private bool ConfirmSaveIfNeeded()
    {
        if (!_hasUnsavedChanges) return true;

        var res = MessageBox.Show(this,
            "Es gibt ungespeicherte Änderungen. Vor dem Fortfahren speichern?",
            "Ungespeicherte Änderungen",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        if (res == DialogResult.Cancel) return false;
        if (res == DialogResult.No) return true;

        SaveAll();
        return !_hasUnsavedChanges;
    }

    private void RefreshListAndReload()
    {
        if (!ConfirmSaveIfNeeded()) return;
        _selectedLangFileName = null;
        if (!EnsureSelectedLangFile(promptIfMultiple: true))
        {
            MessageBox.Show(this, "Keine .lang-Datei gefunden/ausgewählt.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        LoadAll();
    }

    private void BrowseFolder()
    {
        if (!ConfirmSaveIfNeeded()) return;

        using var dlg = new FolderBrowserDialog();
        if (Directory.Exists(_rootPath)) dlg.InitialDirectory = _rootPath;
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _rootPath = dlg.SelectedPath;
        _selectedLangFileName = null;
        if (!EnsureSelectedLangFile(promptIfMultiple: true))
        {
            SaveStartupPath();
            MessageBox.Show(this, "Im gewählten Ordner wurden keine .lang-Dateien gefunden.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        SaveStartupPath();
        LoadAll();
    }

    private void LoadAll()
    {
        try
        {
            var root = _rootPath.Trim();
            if (!Directory.Exists(root)) throw new DirectoryNotFoundException("Languages-Ordner nicht gefunden.");
            if (!EnsureSelectedLangFile(promptIfMultiple: true)) throw new InvalidOperationException("Keine .lang-Datei ausgewählt.");
            SaveStartupPath();

            var fileName = _selectedLangFileName!;
            _languages = Directory.GetDirectories(root)
                .Select(Path.GetFileName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .OrderBy(x => x)
                .ToList();

            _langData.Clear();
            foreach (var lang in _languages)
            {
                var path = Path.Combine(root, lang, fileName);
                _langData[lang] = File.Exists(path) ? ParseLangFile(path) : new Dictionary<string, string>(StringComparer.Ordinal);
            }

            BuildTree();
            BuildEditors();
            SelectFirstKeyIfAny();
            _hasUnsavedChanges = false;
            UpdateStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Laden fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static Dictionary<string, string> ParseLangFile(string path)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var raw in File.ReadAllLines(path, Encoding.UTF8))
        {
            var line = raw.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var idx = line.IndexOf('=');
            if (idx <= 0) continue;
            var key = line[..idx].Trim();
            var value = line[(idx + 1)..].Trim();
            if (!dict.ContainsKey(key)) dict[key] = value;
        }
        return dict;
    }

    private void BuildTree()
    {
        _tree.BeginUpdate();
        _tree.Nodes.Clear();

        var keys = GetFilteredKeys();
        foreach (var key in keys)
        {
            var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            TreeNodeCollection current = _tree.Nodes;
            TreeNode? node = null;

            foreach (var part in parts)
            {
                node = current.Cast<TreeNode>().FirstOrDefault(n => n.Text == part);
                if (node == null)
                {
                    node = new TreeNode(part);
                    current.Add(node);
                }
                current = node.Nodes;
            }

            if (node != null) node.Tag = key;
        }

        _tree.ExpandAll();
        _tree.EndUpdate();
    }

    private List<string> GetFilteredKeys()
    {
        var allKeys = _langData.Values
            .SelectMany(d => d.Keys)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(k => k)
            .ToList();

        var term = _txtSearch.Text.Trim();
        var onlyMissing = _chkOnlyMissing.Checked;

        return allKeys.Where(key =>
            (string.IsNullOrWhiteSpace(term) || key.Contains(term, StringComparison.OrdinalIgnoreCase)) &&
            (!onlyMissing || IsMissingKey(key))
        ).ToList();
    }

    private bool IsMissingKey(string key)
    {
        foreach (var lang in _languages)
        {
            if (!_langData.TryGetValue(lang, out var dict) || !dict.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                return true;
        }

        return false;
    }

    private void ApplyFilters()
    {
        var keepSelected = _selectedKey;
        BuildTree();

        if (!string.IsNullOrWhiteSpace(keepSelected))
            SelectNodeByKey(keepSelected);

        if (_tree.SelectedNode is null)
            SelectFirstKeyIfAny();

        UpdateStatus();
    }

    private void SelectNextMissingKey()
    {
        if (_languages.Count == 0)
        {
            MessageBox.Show(this, "Bitte zuerst eine Sprachdatei laden.");
            return;
        }

        var keys = GetFilteredKeys();
        if (keys.Count == 0)
        {
            UpdateStatus("Keine Keys im aktuellen Filter.");
            return;
        }

        var currentIndex = _selectedKey is null ? -1 : keys.FindIndex(k => string.Equals(k, _selectedKey, StringComparison.Ordinal));
        for (int offset = 1; offset <= keys.Count; offset++)
        {
            var idx = (currentIndex + offset) % keys.Count;
            var key = keys[idx];
            if (!IsMissingKey(key)) continue;

            SelectNodeByKey(key);
            UpdateStatus($"NÃ¤chstes fehlendes Feld: {key}");
            return;
        }

        UpdateStatus("Keine fehlenden Ãœbersetzungen im aktuellen Filter.");
    }

    private void BuildEditors()
    {
        _editorRows.Controls.Clear();
        _editorRows.RowStyles.Clear();
        _langEditors.Clear();

        _editorRows.ColumnStyles.Clear();
        _editorRows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        _editorRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _editorRows.RowCount = _languages.Count;

        for (int i = 0; i < _languages.Count; i++)
        {
            var lang = _languages[i];
            _editorRows.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));

            var lbl = new Label
            {
                Text = lang,
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 6, 0, 0)
            };

            var txt = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Tag = lang,
                Margin = new Padding(0, 0, 0, 10)
            };
            txt.TextChanged += EditorTextChanged;

            _editorRows.Controls.Add(lbl, 0, i);
            _editorRows.Controls.Add(txt, 1, i);
            _langEditors[lang] = txt;
        }
    }

    private void SelectFirstKeyIfAny()
    {
        var firstLeaf = FindFirstLeaf(_tree.Nodes);
        if (firstLeaf != null)
        {
            _tree.SelectedNode = firstLeaf;
            firstLeaf.EnsureVisible();
        }
        else
        {
            _selectedKey = null;
            _lblSelectedKey.Text = "Key: -";
            foreach (var tb in _langEditors.Values) tb.Text = string.Empty;
        }
    }

    private static TreeNode? FindFirstLeaf(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Nodes.Count == 0 && node.Tag is string) return node;
            var child = FindFirstLeaf(node.Nodes);
            if (child != null) return child;
        }
        return null;
    }

    private void OnTreeSelectionChanged(TreeNode? node)
    {
        if (node?.Tag is not string key) return;
        _selectedKey = key;
        _lblSelectedKey.Text = $"Key: {key}";

        _isBinding = true;
        try
        {
            foreach (var lang in _languages)
            {
                _langEditors[lang].Text = _langData[lang].TryGetValue(key, out var value) ? value : string.Empty;
            }
        }
        finally
        {
            _isBinding = false;
        }
    }

    private void EditorTextChanged(object? sender, EventArgs e)
    {
        if (_isBinding || _selectedKey is null || sender is not TextBox tb || tb.Tag is not string lang) return;
        if (!_langData.TryGetValue(lang, out var dict)) return;

        dict[_selectedKey] = tb.Text ?? string.Empty;
        _hasUnsavedChanges = true;
        UpdateStatus();
    }

    private void AddKey()
    {
        var key = Microsoft.VisualBasic.Interaction.InputBox("Neuer Key:", "Key hinzufÃ¼gen", "Items.MyMod.NewKey.name");
        if (string.IsNullOrWhiteSpace(key)) return;
        key = key.Trim();

        var exists = _langData.Values.Any(d => d.ContainsKey(key));
        if (exists)
        {
            MessageBox.Show(this, "Key existiert bereits.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        foreach (var lang in _languages)
        {
            _langData[lang][key] = string.Empty;
        }

        BuildTree();
        SelectNodeByKey(key);
        _hasUnsavedChanges = true;
        UpdateStatus();
    }

    private void DeleteKey()
    {
        if (_selectedKey is null)
        {
            MessageBox.Show(this, "Bitte erst einen Key auswÃ¤hlen.");
            return;
        }

        var res = MessageBox.Show(this, $"Key lÃ¶schen?\n{_selectedKey}", "LÃ¶schen", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (res != DialogResult.Yes) return;

        foreach (var lang in _languages) _langData[lang].Remove(_selectedKey);

        BuildTree();
        SelectFirstKeyIfAny();
        _hasUnsavedChanges = true;
        UpdateStatus();
    }

    private void SelectNodeByKey(string key)
    {
        var node = FindNodeByKey(_tree.Nodes, key);
        if (node == null) return;
        _tree.SelectedNode = node;
        node.EnsureVisible();
    }

    private static TreeNode? FindNodeByKey(TreeNodeCollection nodes, string key)
    {
        foreach (TreeNode n in nodes)
        {
            if (n.Tag is string k && string.Equals(k, key, StringComparison.Ordinal)) return n;
            var c = FindNodeByKey(n.Nodes, key);
            if (c != null) return c;
        }
        return null;
    }

    private void SaveAll()
    {
        try
        {
            if (_languages.Count == 0) throw new InvalidOperationException("Bitte zuerst laden.");
            var root = _rootPath.Trim();
            if (!EnsureSelectedLangFile(promptIfMultiple: true)) throw new InvalidOperationException("Dateiname fehlt.");
            var fileName = _selectedLangFileName!;

            var keys = _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal).OrderBy(k => k).ToList();
            foreach (var lang in _languages)
            {
                var langDir = Path.Combine(root, lang);
                Directory.CreateDirectory(langDir);
                var path = Path.Combine(langDir, fileName);

                using var sw = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                foreach (var key in keys)
                {
                    sw.WriteLine($"{key} = {_langData[lang].GetValueOrDefault(key, string.Empty)}");
                }
            }

            _hasUnsavedChanges = false;
            UpdateStatus("Gespeichert.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Speichern fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportJson()
    {
        try
        {
            if (_languages.Count == 0) throw new InvalidOperationException("Bitte zuerst laden.");

            using var dlg = new SaveFileDialog
            {
                Filter = "JSON (*.json)|*.json|Alle Dateien (*.*)|*.*",
                FileName = (_selectedLangFileName ?? "languages") + ".json",
                Title = "Language JSON exportieren"
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
            UpdateStatus($"JSON exportiert: {Path.GetFileName(dlg.FileName)}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "JSON Export fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ImportJson()
    {
        try
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "JSON (*.json)|*.json|Alle Dateien (*.*)|*.*",
                Title = "Language JSON importieren"
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var json = File.ReadAllText(dlg.FileName, Encoding.UTF8);
            var import = JsonSerializer.Deserialize<JsonLanguageExport>(json)
                ?? throw new InvalidOperationException("UngÃ¼ltige JSON-Datei.");

            var incomingLangs = import.Languages?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? [];

            if (incomingLangs.Count == 0) throw new InvalidOperationException("JSON enthÃ¤lt keine Sprachen.");
            if (import.Entries is null || import.Entries.Count == 0) throw new InvalidOperationException("JSON enthÃ¤lt keine EintrÃ¤ge.");

            var preview = BuildImportPreview(import, incomingLangs);
            if (!preview.CanImport)
            {
                MessageBox.Show(this, preview.Message, "JSON Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var proceed = MessageBox.Show(this,
                preview.Message + "\n\nImport fortsetzen?",
                "JSON Preview / Validation",
                MessageBoxButtons.YesNo,
                preview.WarningCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            if (proceed != DialogResult.Yes) return;

            var replace = MessageBox.Show(this,
                "Bestehende Daten ersetzen?\nJa = ersetzen, Nein = zusammenfÃ¼hren",
                "JSON Import",
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

            foreach (var kv in preview.ValidEntries)
            {
                var key = kv.Key;
                var values = kv.Value;

                foreach (var lang in _languages)
                {
                    if (!_langData.TryGetValue(lang, out var dict))
                    {
                        dict = new Dictionary<string, string>(StringComparer.Ordinal);
                        _langData[lang] = dict;
                    }

                    dict[key] = values.TryGetValue(lang, out var value)
                        ? (value ?? string.Empty)
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
            UpdateStatus($"JSON importiert: {Path.GetFileName(dlg.FileName)}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "JSON Import fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        foreach (var kv in import.Entries)
        {
            var rawKey = kv.Key;
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

            var sourceValues = kv.Value;
            if (sourceValues is null)
            {
                nullValueMaps++;
                sourceValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var normalized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var lang in incomingLangs)
            {
                if (sourceValues.TryGetValue(lang, out var value))
                {
                    normalized[lang] = value ?? string.Empty;
                }
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
            $"Datei: {import.FileName}",
            $"Sprachen: {incomingLangs.Count} ({string.Join(", ", incomingLangs)})",
            $"EintrÃ¤ge gesamt: {import.Entries.Count}",
            $"EintrÃ¤ge importierbar: {validEntries.Count}"
        };

        var warnings = new List<string>();
        if (emptyKeyCount > 0) warnings.Add($"Leere/ungÃ¼ltige Keys Ã¼bersprungen: {emptyKeyCount}");
        if (duplicateKeyCount > 0) warnings.Add($"Doppelte Keys (nach Trim) Ã¼bersprungen: {duplicateKeyCount}");
        if (nullValueMaps > 0) warnings.Add($"EintrÃ¤ge ohne Sprachwerte-Objekt: {nullValueMaps}");
        if (missingValueCount > 0) warnings.Add($"Fehlende Sprachwerte (werden leer importiert): {missingValueCount}");
        if (unknownLangs.Count > 0)
        {
            var sample = string.Join(", ", unknownLangs.Take(8));
            warnings.Add($"Unbekannte Sprachcodes in Entries ignoriert: {sample}{(unknownLangs.Count > 8 ? " ..." : string.Empty)}");
        }

        if (warnings.Count > 0)
        {
            lines.Add(string.Empty);
            lines.Add("Validation-Hinweise:");
            lines.AddRange(warnings.Select(w => "- " + w));
        }

        if (validEntries.Count == 0)
        {
            lines.Add(string.Empty);
            lines.Add("Keine gÃ¼ltigen EintrÃ¤ge zum Import gefunden.");
        }

        return new ImportPreview
        {
            CanImport = validEntries.Count > 0,
            WarningCount = warnings.Count,
            Message = string.Join(Environment.NewLine, lines),
            ValidEntries = validEntries
        };
    }

    private async Task AutoTranslateSelectedKeyAsync()
    {
        try
        {
            if (_selectedKey is null)
            {
                MessageBox.Show(this, "Bitte zuerst einen Key auswÃ¤hlen.");
                return;
            }

            if (_languages.Count < 2)
            {
                MessageBox.Show(this, "Mindestens 2 Sprachen nÃ¶tig.");
                return;
            }

            // optional URL override
            var customUrl = Microsoft.VisualBasic.Interaction.InputBox(
                "LibreTranslate URL (leer lassen = Standard)",
                "Auto-Ãœbersetzen",
                _translateUrl);
            if (!string.IsNullOrWhiteSpace(customUrl)) _translateUrl = NormalizeTranslateUrl(customUrl.Trim());

            var sourceLang = _languages.FirstOrDefault(l => !string.IsNullOrWhiteSpace(_langEditors[l].Text));
            if (sourceLang is null)
            {
                MessageBox.Show(this, "Keine QuellÃ¼bersetzung vorhanden.");
                return;
            }

            var sourceText = _langEditors[sourceLang].Text.Trim();
            var sourceCode = LangCode(sourceLang);
            var changed = 0;

            Cursor = Cursors.WaitCursor;
            _btnAutoTranslate.Enabled = false;

            foreach (var targetLang in _languages)
            {
                if (targetLang.Equals(sourceLang, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrWhiteSpace(_langEditors[targetLang].Text)) continue;

                var translated = await TranslateLibreAsync(sourceText, sourceCode, LangCode(targetLang));
                if (string.IsNullOrWhiteSpace(translated)) continue;

                _langEditors[targetLang].Text = translated;
                _langData[targetLang][_selectedKey] = translated;
                changed++;
            }

            if (changed > 0) _hasUnsavedChanges = true;
            UpdateStatus(changed > 0 ? $"Auto-Ãœbersetzt: {changed} Feld(er)." : "Nichts zu Ã¼bersetzen.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Auto-Ãœbersetzen fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
            _btnAutoTranslate.Enabled = true;
        }
    }

    private static string LangCode(string lang)
    {
        if (string.IsNullOrWhiteSpace(lang)) return "en";
        var idx = lang.IndexOf('-');
        return (idx > 0 ? lang[..idx] : lang).ToLowerInvariant();
    }

    private async Task<string> TranslateLibreAsync(string text, string source, string target)
    {
        var endpoints = new[]
        {
            _translateUrl,
            "https://libretranslate.de/translate",
            "https://translate.argosopentech.com/translate"
        }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

        string? lastError = null;

        foreach (var endpoint in endpoints)
        {
            foreach (var src in new[] { source, "auto" }.Distinct())
            {
                var payload = new Dictionary<string, object>
                {
                    ["q"] = text,
                    ["source"] = src,
                    ["target"] = target,
                    ["format"] = "text"
                };

                using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                string body;
                try
                {
                    response = await _http.PostAsync(endpoint, content);
                    body = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    lastError = $"{endpoint} -> Verbindungsfehler: {ex.Message}";
                    continue;
                }

                using (response)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        lastError = $"{endpoint} -> {(int)response.StatusCode} {response.ReasonPhrase}: {ExtractError(body)}";
                        continue;
                    }
                }

                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("translatedText", out var t))
                    {
                        _translateUrl = endpoint; // remember working endpoint
                        return t.GetString() ?? string.Empty;
                    }

                    lastError = $"{endpoint} -> Antwort ohne translatedText";
                }
                catch
                {
                    lastError = $"{endpoint} -> UngÃ¼ltige JSON-Antwort: {ExtractError(body)}";
                    continue;
                }
            }
        }

        // Final fallback: MyMemory (kostenloses Kontingent)
        try
        {
            var mm = await TranslateMyMemoryAsync(text, source, target);
            if (!string.IsNullOrWhiteSpace(mm)) return mm;
        }
        catch (Exception ex)
        {
            lastError = $"MyMemory Fallback fehlgeschlagen: {ex.Message}";
        }

        throw new InvalidOperationException(lastError ?? "Kein funktionierender Ãœbersetzungs-Endpunkt gefunden.");
    }

    private static string NormalizeTranslateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;
        url = url.Trim().TrimEnd('/');
        if (!url.EndsWith("/translate", StringComparison.OrdinalIgnoreCase))
            url += "/translate";
        return url;
    }

    private async Task<string> TranslateMyMemoryAsync(string text, string source, string target)
    {
        var src = LangCode(source);
        var tgt = LangCode(target);
        var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={src}|{tgt}";

        using var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("responseData", out var rd) && rd.TryGetProperty("translatedText", out var tt))
            return tt.GetString() ?? string.Empty;

        return string.Empty;
    }

    private static string ExtractError(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return "(leere Fehlermeldung)";
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var err)) return err.GetString() ?? body;
            if (doc.RootElement.TryGetProperty("message", out var msg)) return msg.GetString() ?? body;
        }
        catch { }
        return body.Length > 300 ? body[..300] + "..." : body;
    }

    private void UpdateStatus(string? overrideText = null)
    {
        if (!string.IsNullOrWhiteSpace(overrideText))
        {
            _lblStatus.Text = overrideText;
            return;
        }

        if (_languages.Count == 0)
        {
            _lblStatus.Text = "Bereit.";
            return;
        }

        var keyCount = _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal).Count();
        var missing = 0;
        foreach (var key in _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal))
        {
            foreach (var lang in _languages)
            {
                if (!_langData[lang].TryGetValue(key, out var v) || string.IsNullOrWhiteSpace(v)) missing++;
            }
        }

        _lblStatus.Text = $"Sprachen: {_languages.Count} | Keys: {keyCount} | Fehlende Ãœbersetzungen: {missing}";
    }

    private sealed class AppState
    {
        public string RootPath { get; set; } = string.Empty;
        public string SelectedLangFileName { get; set; } = string.Empty;
    }

    private sealed class JsonLanguageExport
    {
        public string FileName { get; set; } = string.Empty;
        public List<string> Languages { get; set; } = [];
        public Dictionary<string, Dictionary<string, string>> Entries { get; set; } = new(StringComparer.Ordinal);
    }

    private sealed class ImportPreview
    {
        public bool CanImport { get; set; }
        public int WarningCount { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, Dictionary<string, string>> ValidEntries { get; set; } = new(StringComparer.Ordinal);
    }
}



