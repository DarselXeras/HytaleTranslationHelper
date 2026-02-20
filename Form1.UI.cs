namespace LanguageFileEditor;

public partial class Form1
{
    private void WireEvents()
    {
        _btnAddKey.Click += (_, _) => AddKey();
        _btnDeleteKey.Click += (_, _) => DeleteKey();
        _btnToggleTreeWidth.Click += (_, _) => ToggleTreeWidth();
        _btnAutoTranslate.Click += async (_, _) => await AutoTranslateSelectedKeyAsync();
        _btnApplyFilter.Click += (_, _) => ApplyFilters();
        _btnNextMissing.Click += (_, _) => SelectNextMissingKey();
        _btnExportJson.Click += (_, _) => ExportJson();
        _btnImportJson.Click += (_, _) => ImportJson();

        _mnuDateiOrdner.Click += (_, _) => BrowseFolder();
        _mnuDateiDateiWaehlen.Click += (_, _) => SelectLanguageFileAndLoad();
        _mnuDateiNeueSprache.Click += (_, _) => CreateLanguageInFolder();
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
        _mnuLangDe.Click += (_, _) => SetLanguage("de");
        _mnuLangEn.Click += (_, _) => SetLanguage("en");

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

    private void ToggleTreeWidth() => _middle.SplitterDistance = _middle.SplitterDistance <= 260 ? 360 : 220;

    private void ApplyFormIcon()
    {
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "LanguageFileEditor_icon.ico");
            if (File.Exists(iconPath)) Icon = new Icon(iconPath);
        }
        catch { }
    }

    private void SetLanguage(string lang)
    {
        _uiLanguage = string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "de";
        ApplyLanguage();
        SaveStartupPath();
    }

    private void ApplyLanguage()
    {
        Text = S("app.title");
        _mnuDatei.Text = S("menu.file");
        _mnuDateiOrdner.Text = S("menu.file.chooseFolder");
        _mnuDateiDateiWaehlen.Text = S("menu.file.chooseFile");
        _mnuDateiNeueSprache.Text = S("menu.file.createLanguage");
        _mnuDateiLaden.Text = S("menu.file.refresh");
        _mnuDateiSpeichern.Text = S("menu.file.save");
        _mnuDateiJsonImport.Text = S("menu.file.import");
        _mnuDateiJsonExport.Text = S("menu.file.export");
        _mnuDateiBeenden.Text = S("menu.file.exit");
        _mnuBearbeiten.Text = S("menu.edit");
        _mnuBearbeitenAddKey.Text = S("menu.edit.addKey");
        _mnuBearbeitenDeleteKey.Text = S("menu.edit.deleteKey");
        _mnuBearbeitenNextMissing.Text = S("menu.edit.nextMissing");
        _mnuBearbeitenCopyPath.Text = S("menu.edit.copyPath");
        _mnuTools.Text = S("menu.tools");
        _mnuToolsAutoTranslate.Text = S("menu.tools.autoTranslate");
        _mnuAnsicht.Text = S("menu.view");
        _mnuAnsichtToggleTreeWidth.Text = S("menu.view.toggleTree");
        _mnuSprache.Text = S("menu.language");
        _mnuLangDe.Text = "Deutsch";
        _mnuLangEn.Text = "English";
        _mnuLangDe.Checked = _uiLanguage == "de";
        _mnuLangEn.Checked = _uiLanguage == "en";
        _lblSearch.Text = S("filter.title");
        _txtSearch.PlaceholderText = S("filter.placeholder");
        _chkOnlyMissing.Text = S("filter.onlyMissing");
        _btnApplyFilter.Text = S("filter.apply");
        _btnNextMissing.Text = S("filter.nextMissing");
        _btnAutoTranslate.Text = S("filter.autoTranslate");
        _miCopyPathAtNode.Text = S("menu.edit.copyPath");
        _miAddEntryAtNode.Text = S("addEntry.context");
        if (_languages.Count == 0) _lblStatus.Text = S("status.ready");
    }

    private void TreeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete) { e.Handled = true; DeleteKey(); return; }
        if (e.KeyCode == Keys.F3) { e.Handled = true; SelectNextMissingKey(); return; }
        if (e.Control && e.KeyCode == Keys.F) { e.Handled = true; _txtSearch.Focus(); _txtSearch.SelectAll(); }
    }

    private void SearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.Handled = true; e.SuppressKeyPress = true; ApplyFilters();
    }

    private void TreeNodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button == MouseButtons.Right) _tree.SelectedNode = e.Node;
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
        var node = _tree.SelectedNode; if (node is null) return;
        var path = GetNodePath(node); if (string.IsNullOrWhiteSpace(path)) return;
        Clipboard.SetText(path);
        UpdateStatus(SF("status.copiedPath", path));
    }
}
