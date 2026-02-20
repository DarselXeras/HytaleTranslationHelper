using System.Text;
using System.Text.Json;

namespace LanguageFileEditor;

public partial class Form1
{
    private void AddEntryAtSelectedNode()
    {
        if (_languages.Count == 0) { MessageBox.Show(this, S("msg.loadFirst")); return; }
        var node = _tree.SelectedNode;
        if (node is null) { MessageBox.Show(this, S("msg.selectNode")); return; }
        var prefixNode = node.Tag is string ? node.Parent : node;
        if (prefixNode is null) { MessageBox.Show(this, S("msg.selectNonRootLeaf")); return; }
        var prefix = GetNodePath(prefixNode);
        if (string.IsNullOrWhiteSpace(prefix)) { MessageBox.Show(this, S("msg.nodePathUnknown")); return; }

        var suffix = Microsoft.VisualBasic.Interaction.InputBox(SF("addEntry.prompt", prefix), S("addEntry.title"), S("addEntry.defaultSuffix"));
        if (string.IsNullOrWhiteSpace(suffix)) return;
        suffix = suffix.Trim();
        if (suffix.Contains('.')) { MessageBox.Show(this, S("msg.suffixNoDot"), S("common.notice")); return; }

        var newKey = $"{prefix}.{suffix}";
        if (_langData.Values.Any(d => d.ContainsKey(newKey))) { MessageBox.Show(this, SF("msg.keyExistsWithName", newKey), S("common.info")); return; }
        foreach (var lang in _languages) _langData[lang][newKey] = string.Empty;
        BuildTree(); SelectNodeByKey(newKey); _hasUnsavedChanges = true; UpdateStatus();
    }

    private static string GetNodePath(TreeNode node)
    {
        var parts = new Stack<string>(); var current = node;
        while (current is not null) { parts.Push(current.Text); current = current.Parent; }
        return string.Join('.', parts);
    }

    private void LoadStartupPath()
    {
        try
        {
            var state = LoadState();
            _rootPath = !string.IsNullOrWhiteSpace(state?.RootPath) ? state!.RootPath : DefaultLanguagesPath;
            _selectedLangFileName = string.IsNullOrWhiteSpace(state?.SelectedLangFileName) ? null : state!.SelectedLangFileName;
            _uiLanguage = string.Equals(state?.UiLanguage, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "de";
        }
        catch { _rootPath = DefaultLanguagesPath; _selectedLangFileName = null; _uiLanguage = "de"; }
    }

    private void SaveStartupPath()
    {
        try
        {
            var dir = Path.GetDirectoryName(_stateFilePath);
            if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(new AppState { RootPath = _rootPath, SelectedLangFileName = _selectedLangFileName ?? string.Empty, UiLanguage = _uiLanguage }, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_stateFilePath, json, Encoding.UTF8);
        }
        catch { }
    }

    private static AppState? LoadState()
    {
        if (!File.Exists(_stateFilePath)) return null;
        var json = File.ReadAllText(_stateFilePath, Encoding.UTF8);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<AppState>(json);
    }

    private List<string> GetAvailableLangFiles(string root)
    {
        if (!Directory.Exists(root)) return [];
        return Directory.GetDirectories(root).SelectMany(langDir => Directory.GetFiles(langDir, "*.lang").Select(Path.GetFileName)).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
    }

    private bool EnsureSelectedLangFile(bool promptIfMultiple = false)
    {
        var files = GetAvailableLangFiles(_rootPath);
        if (files.Count == 0) { _selectedLangFileName = null; return false; }
        if (!string.IsNullOrWhiteSpace(_selectedLangFileName) && files.Contains(_selectedLangFileName, StringComparer.OrdinalIgnoreCase)) return true;
        if (files.Count == 1 || !promptIfMultiple) { _selectedLangFileName = files[0]; SaveStartupPath(); return true; }
        var selected = ChooseLanguageFile(files, files[0]);
        if (selected is null) return false;
        _selectedLangFileName = selected; SaveStartupPath(); return true;
    }

    private string? ChooseLanguageFile(IReadOnlyList<string> files, string? preselect = null)
    {
        using var dlg = new Form { Text = S("menu.file.chooseFile"), Width = 520, Height = 420, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.Sizable, MinimizeBox = false, MaximizeBox = false };
        var list = new ListBox { Dock = DockStyle.Fill }; list.Items.AddRange(files.Cast<object>().ToArray());
        if (!string.IsNullOrWhiteSpace(preselect)) { var idx = files.ToList().FindIndex(x => string.Equals(x, preselect, StringComparison.OrdinalIgnoreCase)); if (idx >= 0) list.SelectedIndex = idx; }
        if (list.SelectedIndex < 0 && list.Items.Count > 0) list.SelectedIndex = 0;
        var btnOk = new Button { Text = S("common.ok"), DialogResult = DialogResult.OK, AutoSize = true };
        var btnCancel = new Button { Text = S("common.cancel"), DialogResult = DialogResult.Cancel, AutoSize = true };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        buttons.Controls.Add(btnOk); buttons.Controls.Add(btnCancel);
        list.DoubleClick += (_, _) => { if (list.SelectedItem is not null) dlg.DialogResult = DialogResult.OK; };
        dlg.AcceptButton = btnOk; dlg.CancelButton = btnCancel; dlg.Controls.Add(list); dlg.Controls.Add(buttons);
        return dlg.ShowDialog(this) == DialogResult.OK ? list.SelectedItem?.ToString() : null;
    }

    private bool ConfirmSaveIfNeeded()
    {
        if (!_hasUnsavedChanges) return true;
        var res = MessageBox.Show(this, S("msg.unsavedChangesQuestion"), S("msg.unsavedChangesTitle"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (res == DialogResult.Cancel) return false;
        if (res == DialogResult.No) return true;
        SaveAll();
        return !_hasUnsavedChanges;
    }

    private void SelectLanguageFileAndLoad()
    {
        if (!ConfirmSaveIfNeeded()) return;
        var files = GetAvailableLangFiles(_rootPath);
        if (files.Count == 0) { MessageBox.Show(this, S("msg.noLangFile"), S("common.notice")); return; }
        var selected = ChooseLanguageFile(files, _selectedLangFileName ?? files[0]); if (string.IsNullOrWhiteSpace(selected)) return;
        _selectedLangFileName = selected; SaveStartupPath(); LoadAll();
    }

    private void RefreshListAndReload()
    {
        if (!ConfirmSaveIfNeeded()) return;
        _selectedLangFileName = null;
        if (!EnsureSelectedLangFile(promptIfMultiple: true)) { MessageBox.Show(this, S("msg.noLangFile"), S("common.notice")); return; }
        LoadAll();
    }

    private void BrowseFolder()
    {
        if (!ConfirmSaveIfNeeded()) return;
        using var dlg = new FolderBrowserDialog();
        if (Directory.Exists(_rootPath)) dlg.InitialDirectory = _rootPath;
        if (dlg.ShowDialog() != DialogResult.OK) return;
        _rootPath = dlg.SelectedPath; _selectedLangFileName = null;
        if (!EnsureSelectedLangFile(promptIfMultiple: true)) { SaveStartupPath(); MessageBox.Show(this, S("msg.noLangFilesInFolder"), S("common.notice")); return; }
        SaveStartupPath(); LoadAll();
    }

    private void LoadAll()
    {
        try
        {
            var root = _rootPath.Trim();
            if (!Directory.Exists(root)) throw new DirectoryNotFoundException(S("err.languagesFolderMissing"));
            if (!EnsureSelectedLangFile(promptIfMultiple: true)) throw new InvalidOperationException(S("err.noLangFileSelected"));
            SaveStartupPath();
            var fileName = _selectedLangFileName!;
            _languages = Directory.GetDirectories(root).Select(Path.GetFileName).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).OrderBy(x => x).ToList();
            _langData.Clear();
            foreach (var lang in _languages)
            {
                var path = Path.Combine(root, lang, fileName);
                _langData[lang] = File.Exists(path) ? ParseLangFile(path) : new Dictionary<string, string>(StringComparer.Ordinal);
            }
            BuildTree(); BuildEditors(); SelectFirstKeyIfAny(); _hasUnsavedChanges = false; UpdateStatus();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, S("err.loadingFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private static Dictionary<string, string> ParseLangFile(string path)
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var raw in File.ReadAllLines(path, Encoding.UTF8))
        {
            var line = raw.Trim(); if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var idx = line.IndexOf('='); if (idx <= 0) continue;
            var key = line[..idx].Trim(); var value = line[(idx + 1)..].Trim();
            if (!dict.ContainsKey(key)) dict[key] = value;
        }
        return dict;
    }

    private void BuildTree()
    {
        _tree.BeginUpdate(); _tree.Nodes.Clear();
        foreach (var key in GetFilteredKeys())
        {
            var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            TreeNodeCollection current = _tree.Nodes; TreeNode? node = null;
            foreach (var part in parts)
            {
                node = current.Cast<TreeNode>().FirstOrDefault(n => n.Text == part);
                if (node == null) { node = new TreeNode(part); current.Add(node); }
                current = node.Nodes;
            }
            if (node != null) node.Tag = key;
        }
        _tree.ExpandAll(); _tree.EndUpdate();
    }

    private List<string> GetFilteredKeys()
    {
        var allKeys = _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal).OrderBy(k => k).ToList();
        var term = _txtSearch.Text.Trim(); var onlyMissing = _chkOnlyMissing.Checked;
        return allKeys.Where(key => (string.IsNullOrWhiteSpace(term) || key.Contains(term, StringComparison.OrdinalIgnoreCase)) && (!onlyMissing || IsMissingKey(key))).ToList();
    }

    private bool IsMissingKey(string key)
    {
        foreach (var lang in _languages)
            if (!_langData.TryGetValue(lang, out var dict) || !dict.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value)) return true;
        return false;
    }

    private void ApplyFilters()
    {
        var keepSelected = _selectedKey;
        BuildTree();
        if (!string.IsNullOrWhiteSpace(keepSelected)) SelectNodeByKey(keepSelected);
        if (_tree.SelectedNode is null) SelectFirstKeyIfAny();
        UpdateStatus();
    }

    private void SelectNextMissingKey()
    {
        if (_languages.Count == 0) { MessageBox.Show(this, S("msg.loadFirst")); return; }
        var keys = GetFilteredKeys(); if (keys.Count == 0) { UpdateStatus(S("status.noKeysInFilter")); return; }
        var currentIndex = _selectedKey is null ? -1 : keys.FindIndex(k => string.Equals(k, _selectedKey, StringComparison.Ordinal));
        for (int offset = 1; offset <= keys.Count; offset++)
        {
            var key = keys[(currentIndex + offset) % keys.Count];
            if (!IsMissingKey(key)) continue;
            SelectNodeByKey(key); UpdateStatus(SF("status.nextMissing", key)); return;
        }
        UpdateStatus(S("status.noMissingInFilter"));
    }

    private void BuildEditors()
    {
        _editorRows.Controls.Clear(); _editorRows.RowStyles.Clear(); _editorRows.ColumnStyles.Clear(); _langEditors.Clear();
        _editorRows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95)); _editorRows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); _editorRows.RowCount = _languages.Count;
        for (int i = 0; i < _languages.Count; i++)
        {
            var lang = _languages[i]; _editorRows.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
            var lbl = new Label { Text = lang, TextAlign = ContentAlignment.TopLeft, Dock = DockStyle.Fill, Padding = new Padding(0, 6, 0, 0) };
            var txt = new TextBox { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical, Tag = lang, Margin = new Padding(0, 0, 0, 10) };
            txt.TextChanged += EditorTextChanged;
            _editorRows.Controls.Add(lbl, 0, i); _editorRows.Controls.Add(txt, 1, i); _langEditors[lang] = txt;
        }
    }

    private void SelectFirstKeyIfAny()
    {
        var firstLeaf = FindFirstLeaf(_tree.Nodes);
        if (firstLeaf != null) { _tree.SelectedNode = firstLeaf; firstLeaf.EnsureVisible(); }
        else { _selectedKey = null; _lblSelectedKey.Text = S("selectedKey.empty"); foreach (var tb in _langEditors.Values) tb.Text = string.Empty; }
    }

    private static TreeNode? FindFirstLeaf(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Nodes.Count == 0 && node.Tag is string) return node;
            var child = FindFirstLeaf(node.Nodes); if (child != null) return child;
        }
        return null;
    }

    private void OnTreeSelectionChanged(TreeNode? node)
    {
        if (node?.Tag is not string key) return;
        _selectedKey = key; _lblSelectedKey.Text = SF("selectedKey.label", key);
        _isBinding = true;
        try { foreach (var lang in _languages) _langEditors[lang].Text = _langData[lang].TryGetValue(key, out var value) ? value : string.Empty; }
        finally { _isBinding = false; }
    }

    private void EditorTextChanged(object? sender, EventArgs e)
    {
        if (_isBinding || _selectedKey is null || sender is not TextBox tb || tb.Tag is not string lang) return;
        if (!_langData.TryGetValue(lang, out var dict)) return;
        dict[_selectedKey] = tb.Text ?? string.Empty; _hasUnsavedChanges = true; UpdateStatus();
    }

    private void AddKey()
    {
        var key = Microsoft.VisualBasic.Interaction.InputBox(S("addKey.prompt"), S("addKey.title"), S("addKey.default"));
        if (string.IsNullOrWhiteSpace(key)) return; key = key.Trim();
        if (_langData.Values.Any(d => d.ContainsKey(key))) { MessageBox.Show(this, S("msg.keyExists"), S("common.info")); return; }
        foreach (var lang in _languages) _langData[lang][key] = string.Empty;
        BuildTree(); SelectNodeByKey(key); _hasUnsavedChanges = true; UpdateStatus();
    }

    private void DeleteKey()
    {
        if (_selectedKey is null) { MessageBox.Show(this, S("msg.selectKeyFirst")); return; }
        var res = MessageBox.Show(this, SF("msg.deleteKeyQuestion", _selectedKey), S("common.delete"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (res != DialogResult.Yes) return;
        foreach (var lang in _languages) _langData[lang].Remove(_selectedKey);
        BuildTree(); SelectFirstKeyIfAny(); _hasUnsavedChanges = true; UpdateStatus();
    }

    private void SelectNodeByKey(string key)
    {
        var node = FindNodeByKey(_tree.Nodes, key); if (node == null) return;
        _tree.SelectedNode = node; node.EnsureVisible();
    }

    private static TreeNode? FindNodeByKey(TreeNodeCollection nodes, string key)
    {
        foreach (TreeNode n in nodes)
        {
            if (n.Tag is string k && string.Equals(k, key, StringComparison.Ordinal)) return n;
            var c = FindNodeByKey(n.Nodes, key); if (c != null) return c;
        }
        return null;
    }

    private void SaveAll()
    {
        try
        {
            if (_languages.Count == 0) throw new InvalidOperationException(S("msg.loadFirst"));
            var root = _rootPath.Trim();
            if (!EnsureSelectedLangFile(promptIfMultiple: true)) throw new InvalidOperationException(S("err.fileNameMissing"));
            var fileName = _selectedLangFileName!;
            var keys = _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal).OrderBy(k => k).ToList();
            foreach (var lang in _languages)
            {
                var path = Path.Combine(root, lang, fileName);
                Directory.CreateDirectory(Path.Combine(root, lang));
                using var sw = new StreamWriter(path, false, new UTF8Encoding(false));
                foreach (var key in keys) sw.WriteLine($"{key} = {_langData[lang].GetValueOrDefault(key, string.Empty)}");
            }
            _hasUnsavedChanges = false; UpdateStatus(S("status.saved"));
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, S("err.saveFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
