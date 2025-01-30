using System.Text.Json;
using System.Text.Json.Serialization;

namespace FMSModManager.Core.Services
{
    public class LanguageService
    {
        private readonly string _languageFolder;
        private Dictionary<string, Dictionary<string, string>> _translations;
        private Dictionary<string, LanguageInfo> _availableLanguages;
        public event Action LanguageChanged;

        public string CurrentLanguage { get; private set; } = "zh-CN";

        public Dictionary<string, LanguageInfo> AvailableLanguages => _availableLanguages;

        public class LanguageInfo
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("file")]
            public string File { get; set; }
        }

        public LanguageService(string basePath)
        {
            _languageFolder = Path.Combine(basePath, "Languages");
            CurrentLanguage = "zh-CN";
            _availableLanguages = GetDefaultLanguages();
            LoadAvailableLanguages();
            LoadTranslations();
        }

        private void LoadAvailableLanguages()
        {
            var configFile = Path.Combine(_languageFolder, "languages.json");
            if (File.Exists(configFile))
            {
                try
                {
                    var json = File.ReadAllText(configFile);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var loadedLanguages = JsonSerializer.Deserialize<Dictionary<string, LanguageInfo>>(json, options);
                    
                    if (loadedLanguages != null)
                    {
                        foreach (var lang in loadedLanguages)
                        {
                            _availableLanguages[lang.Key] = lang.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading languages: {ex.Message}");
                }
            }
        }

        private Dictionary<string, LanguageInfo> GetDefaultLanguages()
        {
            return new Dictionary<string, LanguageInfo>
            {
                {
                    "zh-CN", new LanguageInfo
                    {
                        Name = "简体中文",
                        File = "zh-CN.json"
                    }
                },
                {
                    "en-US", new LanguageInfo
                    {
                        Name = "English",
                        File = "en-US.json"
                    }
                }
            };
        }

        private void LoadTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();
            
            if (!Directory.Exists(_languageFolder))
            {
                Directory.CreateDirectory(_languageFolder);
                return;
            }

            try
            {
                var languageInfo = _availableLanguages[CurrentLanguage];
                var languageFile = Path.Combine(_languageFolder, languageInfo.File);
                if (File.Exists(languageFile))
                {
                    var json = File.ReadAllText(languageFile);
                    _translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                }
                else if (CurrentLanguage != "zh-CN")
                {
                    var defaultLanguageInfo = _availableLanguages["zh-CN"];
                    var defaultLanguageFile = Path.Combine(_languageFolder, defaultLanguageInfo.File);
                    if (File.Exists(defaultLanguageFile))
                    {
                        var json = File.ReadAllText(defaultLanguageFile);
                        _translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                        System.Diagnostics.Debug.WriteLine($"Using Chinese translations as fallback for {CurrentLanguage}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (CurrentLanguage != "zh-CN")
                {
                    try
                    {
                        var defaultLanguageInfo = _availableLanguages["zh-CN"];
                        var defaultLanguageFile = Path.Combine(_languageFolder, defaultLanguageInfo.File);
                        if (File.Exists(defaultLanguageFile))
                        {
                            var json = File.ReadAllText(defaultLanguageFile);
                            _translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                            System.Diagnostics.Debug.WriteLine($"Using Chinese translations as fallback after error: {ex.Message}");
                        }
                    }
                    catch (Exception fallbackEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading fallback translations: {fallbackEx.Message}");
                        _translations = new Dictionary<string, Dictionary<string, string>>();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading translations: {ex.Message}");
                    _translations = new Dictionary<string, Dictionary<string, string>>();
                }
            }
        }

        public void SetLanguage(string language)
        {
            if (CurrentLanguage != language && _availableLanguages.ContainsKey(language))
            {
                CurrentLanguage = language;
                LoadTranslations();
                LanguageChanged?.Invoke();
            }
        }

        public string GetText(string key, string category = "Common")
        {
            if (_translations != null &&
                _translations.TryGetValue(category, out var categoryDict) &&
                categoryDict.TryGetValue(key, out var text))
            {
                if (CurrentLanguage == "en-US")
                {
                    return text.Length > 0 ? 
                        char.ToUpper(text[0]) + text.Substring(1).ToLower() : 
                        text;
                }
                return text;
            }
            return key;
        }
    }
} 