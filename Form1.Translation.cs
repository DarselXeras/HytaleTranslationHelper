using System.Text;
using System.Text.Json;

namespace LanguageFileEditor;

public partial class Form1
{
    private async Task AutoTranslateSelectedKeyAsync()
    {
        try
        {
            if (_selectedKey is null)
            {
                MessageBox.Show(this, S("msg.selectKeyFirst"));
                return;
            }

            if (_languages.Count < 2)
            {
                MessageBox.Show(this, S("err.minTwoLanguages"));
                return;
            }

            var customUrl = Microsoft.VisualBasic.Interaction.InputBox(
                S("translate.url.prompt"),
                S("translate.title"),
                _translateUrl);

            if (!string.IsNullOrWhiteSpace(customUrl))
                _translateUrl = NormalizeTranslateUrl(customUrl.Trim());

            var sourceLang = _languages.FirstOrDefault(l => !string.IsNullOrWhiteSpace(_langEditors[l].Text));
            if (sourceLang is null)
            {
                MessageBox.Show(this, S("err.noSourceTranslation"));
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

            if (changed > 0)
                _hasUnsavedChanges = true;

            UpdateStatus(changed > 0 ? SF("status.autoTranslated", changed) : S("status.nothingToTranslate"));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, S("err.autoTranslateFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    lastError = SF("err.translateConnection", endpoint, ex.Message);
                    continue;
                }

                using (response)
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        lastError = SF("err.translateHttp", endpoint, (int)response.StatusCode, response.ReasonPhrase ?? string.Empty, ExtractError(body));
                        continue;
                    }
                }

                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("translatedText", out var translatedTextElement))
                    {
                        _translateUrl = endpoint;
                        return translatedTextElement.GetString() ?? string.Empty;
                    }

                    lastError = SF("err.translateMissingField", endpoint);
                }
                catch
                {
                    lastError = SF("err.translateInvalidJson", endpoint, ExtractError(body));
                }
            }
        }

        try
        {
            var myMemoryText = await TranslateMyMemoryAsync(text, source, target);
            if (!string.IsNullOrWhiteSpace(myMemoryText))
                return myMemoryText;
        }
        catch (Exception ex)
        {
            lastError = SF("err.myMemoryFailed", ex.Message);
        }

        throw new InvalidOperationException(lastError ?? S("err.noTranslateEndpoint"));
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

        if (doc.RootElement.TryGetProperty("responseData", out var responseData) &&
            responseData.TryGetProperty("translatedText", out var translatedTextElement))
        {
            return translatedTextElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string ExtractError(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return "(empty error message)";

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var err)) return err.GetString() ?? body;
            if (doc.RootElement.TryGetProperty("message", out var msg)) return msg.GetString() ?? body;
        }
        catch
        {
            // ignore parse errors
        }

        return body.Length > 300 ? body[..300] + "..." : body;
    }
}
