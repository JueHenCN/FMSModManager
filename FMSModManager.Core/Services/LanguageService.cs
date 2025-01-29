using System.Text.Json;

namespace FMSModManager.Core.Services
{
    public class LanguageService
    {
        private readonly string _languageFolder;
        private Dictionary<string, Dictionary<string, string>> _translations;
        public event Action LanguageChanged;

        public string CurrentLanguage { get; private set; } = "zh-CN";

        public static readonly Dictionary<string, string> AvailableLanguages = new()
        {
            { "zh-CN", "简体中文" },
            { "en-US", "English" }
        };

        public LanguageService(string basePath)
        {
            _languageFolder = Path.Combine(basePath, "Languages");
            LoadTranslations();
        }

        private void LoadTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();
            
            if (!Directory.Exists(_languageFolder))
            {
                Directory.CreateDirectory(_languageFolder);
            }

            var languageFile = Path.Combine(_languageFolder, $"{CurrentLanguage}.json");
            if (File.Exists(languageFile))
            {
                var json = File.ReadAllText(languageFile);
                _translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
            }
        }

        public void SetLanguage(string language)
        {
            if (CurrentLanguage != language && AvailableLanguages.ContainsKey(language))
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
                return text;
            }
            return key;
        }
    }
} 