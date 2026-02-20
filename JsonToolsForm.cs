namespace LanguageFileEditor;

public partial class JsonToolsForm : Form
{
    public event EventHandler? ImportRequested;
    public event EventHandler? ExportRequested;

    public JsonToolsForm()
    {
        InitializeComponent();
    }

    private void BtnImport_Click(object? sender, EventArgs e)
    {
        ImportRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        ExportRequested?.Invoke(this, EventArgs.Empty);
    }
}
