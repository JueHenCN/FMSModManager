using System.Text.Json;
using System.Text.Json.Serialization;
using FMSModManager.Core.Events;
using FMSModManager.Core.Services.Interface;
using Prism.Events;

namespace FMSModManager.Core.Services
{
    public class LanguageService
    {
        private readonly string _languageFolder;
        private readonly IFileService _fileService;
        private readonly LocalConfigService _localConfigService;
        private readonly IEventAggregator _eventAggregator;
        private Dictionary<string, Dictionary<string, string>>? _translations;
        private List<string?> _availableLanguages { get; set; } = new List<string?>();
        public string? CurrentLanguage { get; set; }

        public LanguageService(LocalConfigService localConfigService, IFileService fileService, IEventAggregator eventAggregator)
        {
            _languageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages");
            _fileService = fileService;
            _localConfigService = localConfigService;
            _eventAggregator = eventAggregator;
            _availableLanguages = Directory.GetFiles(_languageFolder, "*.json").Select(Path.GetFileNameWithoutExtension).ToList();
            LoadTranslations(_localConfigService.LocalConfig.SelLanguageName);

        }

        public List<string?> AvailableLanguages => _availableLanguages;

        private void LoadTranslations(string language)
        {
            var languageFile = Path.Combine(_languageFolder, $"{language}.json");
            if (File.Exists(languageFile))
            {
                var json = File.ReadAllText(languageFile);
                _translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>?>(json);
            }
            CurrentLanguage = language;
        }

        public void UpdateLanguage(string language)
        {
            LoadTranslations(language);
            _eventAggregator.GetEvent<LanguageChangedEvent>().Publish(language);
        }

        public string GetText(string key, string category)
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