using System.Globalization;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Text.Json;

namespace LanguageFileEditor;

public partial class Form1 : Form
{
    private readonly Dictionary<string, Dictionary<string, string>> _langData = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, TextBox> _langEditors = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(20) };
    private static readonly ResourceManager _rm = new("LanguageFileEditor.Resources.UIStrings", typeof(Form1).Assembly);

    private const string DefaultLanguagesPath = "";

    private static readonly string _stateFilePath = Path.Combine(
        AppContext.BaseDirectory,
        "config",
        "lfe.config");

    private string _translateUrl = "https://libretranslate.com/translate";
    private string _rootPath = DefaultLanguagesPath;
    private string? _selectedLangFileName;
    private List<string> _languages = [];
    private string? _selectedKey;
    private bool _isBinding;
    private bool _hasUnsavedChanges;
    private string _uiLanguage = "de";

    public Form1()
    {
        InitializeComponent();
        Width = 1450;
        Height = 900;

        WireEvents();
        ApplyFormIcon();

        LoadStartupPath();
        ApplyLanguage();
        EnsureSelectedLangFile();
        UpdateStatus(S("status.ready"));
    }

    private string S(string key)
        => _rm.GetString(key, CultureInfo.GetCultureInfo(_uiLanguage)) ?? key;

    private string SF(string key, params object[] args)
        => string.Format(S(key), args);

    private void UpdateStatus(string? overrideText = null)
    {
        if (!string.IsNullOrWhiteSpace(overrideText))
        {
            _lblStatus.Text = overrideText;
            return;
        }

        if (_languages.Count == 0)
        {
            _lblStatus.Text = S("status.ready");
            return;
        }

        var allKeys = _langData.Values.SelectMany(d => d.Keys).Distinct(StringComparer.Ordinal).ToList();
        var missing = 0;

        foreach (var key in allKeys)
        {
            foreach (var lang in _languages)
            {
                if (!_langData[lang].TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                    missing++;
            }
        }

        _lblStatus.Text = SF("status.summary", _languages.Count, allKeys.Count, missing);
    }

    private sealed class AppState
    {
        public string RootPath { get; set; } = string.Empty;
        public string SelectedLangFileName { get; set; } = string.Empty;
        public string UiLanguage { get; set; } = "de";
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
