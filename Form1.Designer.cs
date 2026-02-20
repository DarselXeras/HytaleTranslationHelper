namespace LanguageFileEditor;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _btnAddKey = new Button();
        _btnDeleteKey = new Button();
        _btnToggleTreeWidth = new Button();
        _btnAutoTranslate = new Button();
        _txtSearch = new TextBox();
        _chkOnlyMissing = new CheckBox();
        _btnApplyFilter = new Button();
        _btnNextMissing = new Button();
        _btnExportJson = new Button();
        _btnImportJson = new Button();
        _treeMenu = new ContextMenuStrip(components);
        _miCopyPathAtNode = new ToolStripMenuItem();
        _miAddEntryAtNode = new ToolStripMenuItem();
        _tree = new TreeView();
        _lblSelectedKey = new Label();
        _editorRows = new TableLayoutPanel();
        _lblStatus = new Label();
        _middle = new SplitContainer();
        _leftPanel = new Panel();
        _rightPanel = new Panel();
        _menuMain = new MenuStrip();
        _mnuDatei = new ToolStripMenuItem();
        _mnuDateiOrdner = new ToolStripMenuItem();
        _mnuDateiDateiWaehlen = new ToolStripMenuItem();
        _mnuDateiNeueSprache = new ToolStripMenuItem();
        _mnuDateiLaden = new ToolStripMenuItem();
        _mnuDateiSpeichern = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        _mnuDateiJsonImport = new ToolStripMenuItem();
        _mnuDateiJsonExport = new ToolStripMenuItem();
        toolStripSeparator2 = new ToolStripSeparator();
        _mnuDateiBeenden = new ToolStripMenuItem();
        _mnuBearbeiten = new ToolStripMenuItem();
        _mnuBearbeitenAddKey = new ToolStripMenuItem();
        _mnuBearbeitenDeleteKey = new ToolStripMenuItem();
        _mnuBearbeitenNextMissing = new ToolStripMenuItem();
        _mnuBearbeitenCopyPath = new ToolStripMenuItem();
        _mnuTools = new ToolStripMenuItem();
        _mnuToolsAutoTranslate = new ToolStripMenuItem();
        _mnuAnsicht = new ToolStripMenuItem();
        _mnuAnsichtToggleTreeWidth = new ToolStripMenuItem();
        _mnuSprache = new ToolStripMenuItem();
        _mnuLangDe = new ToolStripMenuItem();
        _mnuLangEn = new ToolStripMenuItem();
        _filterBar = new TableLayoutPanel();
        _lblSearch = new Label();
        _treeMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_middle).BeginInit();
        _middle.Panel1.SuspendLayout();
        _middle.Panel2.SuspendLayout();
        _middle.SuspendLayout();
        _leftPanel.SuspendLayout();
        _rightPanel.SuspendLayout();
        _menuMain.SuspendLayout();
        _filterBar.SuspendLayout();
        SuspendLayout();
        // 
        // _btnAddKey
        // 
        _btnAddKey.Location = new Point(0, 0);
        _btnAddKey.Name = "_btnAddKey";
        _btnAddKey.Size = new Size(75, 23);
        _btnAddKey.TabIndex = 0;
        _btnAddKey.Visible = false;
        // 
        // _btnDeleteKey
        // 
        _btnDeleteKey.Location = new Point(0, 0);
        _btnDeleteKey.Name = "_btnDeleteKey";
        _btnDeleteKey.Size = new Size(75, 23);
        _btnDeleteKey.TabIndex = 0;
        _btnDeleteKey.Visible = false;
        // 
        // _btnToggleTreeWidth
        // 
        _btnToggleTreeWidth.Location = new Point(0, 0);
        _btnToggleTreeWidth.Name = "_btnToggleTreeWidth";
        _btnToggleTreeWidth.Size = new Size(75, 23);
        _btnToggleTreeWidth.TabIndex = 0;
        _btnToggleTreeWidth.Visible = false;
        // 
        // _btnAutoTranslate
        // 
        _btnAutoTranslate.Dock = DockStyle.Fill;
        _btnAutoTranslate.Location = new Point(1008, 7);
        _btnAutoTranslate.Name = "_btnAutoTranslate";
        _btnAutoTranslate.Size = new Size(154, 24);
        _btnAutoTranslate.TabIndex = 0;
        // 
        // _txtSearch
        // 
        _txtSearch.Dock = DockStyle.Fill;
        _txtSearch.Location = new Point(131, 7);
        _txtSearch.Name = "_txtSearch";
        _txtSearch.PlaceholderText = "Key suchen...";
        _txtSearch.Size = new Size(521, 23);
        _txtSearch.TabIndex = 1;
        // 
        // _chkOnlyMissing
        // 
        _chkOnlyMissing.AutoSize = true;
        _chkOnlyMissing.Dock = DockStyle.Fill;
        _chkOnlyMissing.Location = new Point(658, 7);
        _chkOnlyMissing.Name = "_chkOnlyMissing";
        _chkOnlyMissing.Size = new Size(124, 24);
        _chkOnlyMissing.TabIndex = 2;
        _chkOnlyMissing.Text = "Nur fehlende";
        // 
        // _btnApplyFilter
        // 
        _btnApplyFilter.Dock = DockStyle.Fill;
        _btnApplyFilter.Location = new Point(788, 7);
        _btnApplyFilter.Name = "_btnApplyFilter";
        _btnApplyFilter.Size = new Size(64, 24);
        _btnApplyFilter.TabIndex = 3;
        _btnApplyFilter.Text = "Filter";
        // 
        // _btnNextMissing
        // 
        _btnNextMissing.Dock = DockStyle.Fill;
        _btnNextMissing.Location = new Point(858, 7);
        _btnNextMissing.Name = "_btnNextMissing";
        _btnNextMissing.Size = new Size(144, 24);
        _btnNextMissing.TabIndex = 4;
        _btnNextMissing.Text = "Nächstes fehlendes";
        // 
        // _btnExportJson
        // 
        _btnExportJson.Location = new Point(0, 0);
        _btnExportJson.Name = "_btnExportJson";
        _btnExportJson.Size = new Size(75, 23);
        _btnExportJson.TabIndex = 0;
        _btnExportJson.Visible = false;
        // 
        // _btnImportJson
        // 
        _btnImportJson.Location = new Point(0, 0);
        _btnImportJson.Name = "_btnImportJson";
        _btnImportJson.Size = new Size(75, 23);
        _btnImportJson.TabIndex = 0;
        _btnImportJson.Visible = false;
        // 
        // _treeMenu
        // 
        _treeMenu.Items.AddRange(new ToolStripItem[] { _miCopyPathAtNode, _miAddEntryAtNode });
        _treeMenu.Name = "_treeMenu";
        _treeMenu.Size = new Size(236, 48);
        // 
        // _miCopyPathAtNode
        // 
        _miCopyPathAtNode.Name = "_miCopyPathAtNode";
        _miCopyPathAtNode.Size = new Size(235, 22);
        _miCopyPathAtNode.Text = "Pfad bis hierhin kopieren";
        // 
        // _miAddEntryAtNode
        // 
        _miAddEntryAtNode.Name = "_miAddEntryAtNode";
        _miAddEntryAtNode.Size = new Size(235, 22);
        _miAddEntryAtNode.Text = "Neuen Eintrag hier hinzufügen";
        // 
        // _tree
        // 
        _tree.ContextMenuStrip = _treeMenu;
        _tree.Dock = DockStyle.Fill;
        _tree.HideSelection = false;
        _tree.Location = new Point(0, 0);
        _tree.Name = "_tree";
        _tree.Size = new Size(253, 631);
        _tree.TabIndex = 0;
        // 
        // _lblSelectedKey
        // 
        _lblSelectedKey.Dock = DockStyle.Top;
        _lblSelectedKey.Location = new Point(0, 0);
        _lblSelectedKey.Name = "_lblSelectedKey";
        _lblSelectedKey.Padding = new Padding(8, 0, 8, 0);
        _lblSelectedKey.Size = new Size(916, 28);
        _lblSelectedKey.TabIndex = 1;
        _lblSelectedKey.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _editorRows
        // 
        _editorRows.AutoScroll = true;
        _editorRows.ColumnCount = 2;
        _editorRows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        _editorRows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        _editorRows.Dock = DockStyle.Fill;
        _editorRows.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
        _editorRows.Location = new Point(0, 28);
        _editorRows.Name = "_editorRows";
        _editorRows.Padding = new Padding(8);
        _editorRows.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _editorRows.Size = new Size(916, 603);
        _editorRows.TabIndex = 0;
        // 
        // _lblStatus
        // 
        _lblStatus.Dock = DockStyle.Bottom;
        _lblStatus.Location = new Point(0, 693);
        _lblStatus.Name = "_lblStatus";
        _lblStatus.Padding = new Padding(8, 0, 8, 0);
        _lblStatus.Size = new Size(1173, 24);
        _lblStatus.TabIndex = 2;
        _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _middle
        // 
        _middle.Dock = DockStyle.Fill;
        _middle.FixedPanel = FixedPanel.Panel1;
        _middle.Location = new Point(0, 62);
        _middle.Name = "_middle";
        // 
        // _middle.Panel1
        // 
        _middle.Panel1.Controls.Add(_leftPanel);
        // 
        // _middle.Panel2
        // 
        _middle.Panel2.Controls.Add(_rightPanel);
        _middle.Size = new Size(1173, 631);
        _middle.SplitterDistance = 253;
        _middle.TabIndex = 1;
        // 
        // _leftPanel
        // 
        _leftPanel.Controls.Add(_tree);
        _leftPanel.Dock = DockStyle.Fill;
        _leftPanel.Location = new Point(0, 0);
        _leftPanel.Name = "_leftPanel";
        _leftPanel.Size = new Size(253, 631);
        _leftPanel.TabIndex = 0;
        // 
        // _rightPanel
        // 
        _rightPanel.Controls.Add(_editorRows);
        _rightPanel.Controls.Add(_lblSelectedKey);
        _rightPanel.Dock = DockStyle.Fill;
        _rightPanel.Location = new Point(0, 0);
        _rightPanel.Name = "_rightPanel";
        _rightPanel.Size = new Size(916, 631);
        _rightPanel.TabIndex = 0;
        // 
        // _menuMain
        // 
        _menuMain.Items.AddRange(new ToolStripItem[] { _mnuDatei, _mnuBearbeiten, _mnuTools, _mnuAnsicht, _mnuSprache });
        _menuMain.Location = new Point(0, 0);
        _menuMain.Name = "_menuMain";
        _menuMain.Size = new Size(1173, 24);
        _menuMain.TabIndex = 4;
        // 
        // _mnuDatei
        // 
        _mnuDatei.DropDownItems.AddRange(new ToolStripItem[] { _mnuDateiOrdner, _mnuDateiDateiWaehlen, _mnuDateiNeueSprache, _mnuDateiLaden, _mnuDateiSpeichern, toolStripSeparator1, _mnuDateiJsonImport, _mnuDateiJsonExport, toolStripSeparator2, _mnuDateiBeenden });
        _mnuDatei.Name = "_mnuDatei";
        _mnuDatei.Size = new Size(46, 20);
        _mnuDatei.Text = "Datei";
        // 
        // _mnuDateiOrdner
        // 
        _mnuDateiOrdner.Name = "_mnuDateiOrdner";
        _mnuDateiOrdner.Size = new Size(223, 22);
        _mnuDateiOrdner.Text = "Languages-Ordner wählen...";
        // 
        // _mnuDateiDateiWaehlen
        // 
        _mnuDateiDateiWaehlen.Name = "_mnuDateiDateiWaehlen";
        _mnuDateiDateiWaehlen.Size = new Size(223, 22);
        _mnuDateiDateiWaehlen.Text = "Sprachdatei wählen...";
        // 
        // _mnuDateiNeueSprache
        // 
        _mnuDateiNeueSprache.Name = "_mnuDateiNeueSprache";
        _mnuDateiNeueSprache.Size = new Size(223, 22);
        _mnuDateiNeueSprache.Text = "Neue Sprache anlegen...";
        // 
        // _mnuDateiLaden
        // 
        _mnuDateiLaden.Name = "_mnuDateiLaden";
        _mnuDateiLaden.Size = new Size(223, 22);
        _mnuDateiLaden.Text = "Liste aktualisieren";
        // 
        // _mnuDateiSpeichern
        // 
        _mnuDateiSpeichern.Name = "_mnuDateiSpeichern";
        _mnuDateiSpeichern.Size = new Size(223, 22);
        _mnuDateiSpeichern.Text = "Speichern";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(220, 6);
        // 
        // _mnuDateiJsonImport
        // 
        _mnuDateiJsonImport.Name = "_mnuDateiJsonImport";
        _mnuDateiJsonImport.Size = new Size(223, 22);
        _mnuDateiJsonImport.Text = "JSON importieren...";
        // 
        // _mnuDateiJsonExport
        // 
        _mnuDateiJsonExport.Name = "_mnuDateiJsonExport";
        _mnuDateiJsonExport.Size = new Size(223, 22);
        _mnuDateiJsonExport.Text = "JSON exportieren...";
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(220, 6);
        // 
        // _mnuDateiBeenden
        // 
        _mnuDateiBeenden.Name = "_mnuDateiBeenden";
        _mnuDateiBeenden.Size = new Size(223, 22);
        _mnuDateiBeenden.Text = "Beenden";
        // 
        // _mnuBearbeiten
        // 
        _mnuBearbeiten.DropDownItems.AddRange(new ToolStripItem[] { _mnuBearbeitenAddKey, _mnuBearbeitenDeleteKey, _mnuBearbeitenNextMissing, _mnuBearbeitenCopyPath });
        _mnuBearbeiten.Name = "_mnuBearbeiten";
        _mnuBearbeiten.Size = new Size(75, 20);
        _mnuBearbeiten.Text = "Bearbeiten";
        // 
        // _mnuBearbeitenAddKey
        // 
        _mnuBearbeitenAddKey.Name = "_mnuBearbeitenAddKey";
        _mnuBearbeitenAddKey.Size = new Size(205, 22);
        _mnuBearbeitenAddKey.Text = "Key hinzufügen";
        // 
        // _mnuBearbeitenDeleteKey
        // 
        _mnuBearbeitenDeleteKey.Name = "_mnuBearbeitenDeleteKey";
        _mnuBearbeitenDeleteKey.Size = new Size(205, 22);
        _mnuBearbeitenDeleteKey.Text = "Key löschen";
        // 
        // _mnuBearbeitenNextMissing
        // 
        _mnuBearbeitenNextMissing.Name = "_mnuBearbeitenNextMissing";
        _mnuBearbeitenNextMissing.Size = new Size(205, 22);
        _mnuBearbeitenNextMissing.Text = "Nächstes fehlendes Feld";
        // 
        // _mnuBearbeitenCopyPath
        // 
        _mnuBearbeitenCopyPath.Name = "_mnuBearbeitenCopyPath";
        _mnuBearbeitenCopyPath.Size = new Size(205, 22);
        _mnuBearbeitenCopyPath.Text = "Pfad bis hierhin kopieren";
        // 
        // _mnuTools
        // 
        _mnuTools.DropDownItems.AddRange(new ToolStripItem[] { _mnuToolsAutoTranslate });
        _mnuTools.Name = "_mnuTools";
        _mnuTools.Size = new Size(77, 20);
        _mnuTools.Text = "Werkzeuge";
        // 
        // _mnuToolsAutoTranslate
        // 
        _mnuToolsAutoTranslate.Name = "_mnuToolsAutoTranslate";
        _mnuToolsAutoTranslate.Size = new Size(241, 22);
        _mnuToolsAutoTranslate.Text = "Auto-Übersetzen (aktueller Key)";
        // 
        // _mnuAnsicht
        // 
        _mnuAnsicht.DropDownItems.AddRange(new ToolStripItem[] { _mnuAnsichtToggleTreeWidth });
        _mnuAnsicht.Name = "_mnuAnsicht";
        _mnuAnsicht.Size = new Size(59, 20);
        _mnuAnsicht.Text = "Ansicht";
        // 
        // _mnuAnsichtToggleTreeWidth
        // 
        _mnuAnsichtToggleTreeWidth.Name = "_mnuAnsichtToggleTreeWidth";
        _mnuAnsichtToggleTreeWidth.Size = new Size(205, 22);
        _mnuAnsichtToggleTreeWidth.Text = "Baum-Breite umschalten";
        // 
        // _mnuSprache
        // 
        _mnuSprache.DropDownItems.AddRange(new ToolStripItem[] { _mnuLangDe, _mnuLangEn });
        _mnuSprache.Name = "_mnuSprache";
        _mnuSprache.Size = new Size(61, 20);
        _mnuSprache.Text = "Sprache";
        // 
        // _mnuLangDe
        // 
        _mnuLangDe.Name = "_mnuLangDe";
        _mnuLangDe.Size = new Size(117, 22);
        _mnuLangDe.Text = "Deutsch";
        // 
        // _mnuLangEn
        // 
        _mnuLangEn.Name = "_mnuLangEn";
        _mnuLangEn.Size = new Size(117, 22);
        _mnuLangEn.Text = "English";
        // 
        // _filterBar
        // 
        _filterBar.ColumnCount = 6;
        _filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
        _filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
        _filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        _filterBar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        _filterBar.Controls.Add(_lblSearch, 0, 0);
        _filterBar.Controls.Add(_txtSearch, 1, 0);
        _filterBar.Controls.Add(_chkOnlyMissing, 2, 0);
        _filterBar.Controls.Add(_btnApplyFilter, 3, 0);
        _filterBar.Controls.Add(_btnNextMissing, 4, 0);
        _filterBar.Controls.Add(_btnAutoTranslate, 5, 0);
        _filterBar.Dock = DockStyle.Top;
        _filterBar.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
        _filterBar.Location = new Point(0, 24);
        _filterBar.Name = "_filterBar";
        _filterBar.Padding = new Padding(8, 4, 8, 4);
        _filterBar.RowCount = 1;
        _filterBar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _filterBar.Size = new Size(1173, 38);
        _filterBar.TabIndex = 3;
        // 
        // _lblSearch
        // 
        _lblSearch.Dock = DockStyle.Fill;
        _lblSearch.Location = new Point(11, 4);
        _lblSearch.Name = "_lblSearch";
        _lblSearch.Size = new Size(114, 30);
        _lblSearch.TabIndex = 0;
        _lblSearch.Text = "Suche/Filter";
        _lblSearch.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1173, 717);
        Controls.Add(_middle);
        Controls.Add(_lblStatus);
        Controls.Add(_filterBar);
        Controls.Add(_menuMain);
        MainMenuStrip = _menuMain;
        Name = "Form1";
        Text = "Hytale Languagefile Editor";
        _treeMenu.ResumeLayout(false);
        _middle.Panel1.ResumeLayout(false);
        _middle.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_middle).EndInit();
        _middle.ResumeLayout(false);
        _leftPanel.ResumeLayout(false);
        _rightPanel.ResumeLayout(false);
        _menuMain.ResumeLayout(false);
        _menuMain.PerformLayout();
        _filterBar.ResumeLayout(false);
        _filterBar.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button _btnAddKey;
    private Button _btnDeleteKey;
    private Button _btnToggleTreeWidth;
    private Button _btnAutoTranslate;
    private TextBox _txtSearch;
    private CheckBox _chkOnlyMissing;
    private Button _btnApplyFilter;
    private Button _btnNextMissing;
    private Button _btnExportJson;
    private Button _btnImportJson;
    private ContextMenuStrip _treeMenu;
    private ToolStripMenuItem _miCopyPathAtNode;
    private ToolStripMenuItem _miAddEntryAtNode;

    private TreeView _tree;
    private Label _lblSelectedKey;
    private TableLayoutPanel _editorRows;
    private Label _lblStatus;
    private SplitContainer _middle;

    private TableLayoutPanel _filterBar;
    private Label _lblSearch;
    private Panel _leftPanel;
    private Panel _rightPanel;

    private MenuStrip _menuMain;
    private ToolStripMenuItem _mnuDatei;
    private ToolStripMenuItem _mnuDateiOrdner;
    private ToolStripMenuItem _mnuDateiDateiWaehlen;
    private ToolStripMenuItem _mnuDateiNeueSprache;
    private ToolStripMenuItem _mnuDateiLaden;
    private ToolStripMenuItem _mnuDateiSpeichern;
    private ToolStripMenuItem _mnuDateiJsonImport;
    private ToolStripMenuItem _mnuDateiJsonExport;
    private ToolStripMenuItem _mnuDateiBeenden;
    private ToolStripMenuItem _mnuBearbeiten;
    private ToolStripMenuItem _mnuBearbeitenAddKey;
    private ToolStripMenuItem _mnuBearbeitenDeleteKey;
    private ToolStripMenuItem _mnuBearbeitenNextMissing;
    private ToolStripMenuItem _mnuBearbeitenCopyPath;
    private ToolStripMenuItem _mnuTools;
    private ToolStripMenuItem _mnuToolsAutoTranslate;
    private ToolStripMenuItem _mnuAnsicht;
    private ToolStripMenuItem _mnuSprache;
    private ToolStripMenuItem _mnuLangDe;
    private ToolStripMenuItem _mnuLangEn;
    private ToolStripMenuItem _mnuAnsichtToggleTreeWidth;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripSeparator toolStripSeparator2;
}


