using System.Text.Json;
using System.Text.RegularExpressions;
using FMSModManager.Core.Models;

namespace FMSModManager.Core.Services
{
    public class CultureService
    {
        private readonly string _basePath;
        private readonly Dictionary<string, Culture> _cultures = new();
        private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public CultureService(string basePath)
        {
            _basePath = Path.Combine(basePath, "Culture");
        }

        public List<string> GetAvailableCultures()
        {
            if (!Directory.Exists(_basePath))
            {
                return new List<string>();
            }
            return Directory.GetDirectories(_basePath)
                          .Select(Path.GetFileName)
                          .ToList();
        }
        private List<T> LoadTranslationFile<T>(string filePath) where T : new()
        {
            var results = new List<T>();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length < 2) return results;

            var headers = lines[0].Split(',');
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                if (string.IsNullOrWhiteSpace(values[0])) continue;

                var item = new T();
                var translations = new Dictionary<string, string>();

                for (int j = 1; j < Math.Min(headers.Length, values.Length); j++)
                {
                    if (!string.IsNullOrWhiteSpace(headers[j]) && !string.IsNullOrWhiteSpace(values[j]))
                    {
                        translations[headers[j]] = values[j];
                    }
                }

                var keyProperty = typeof(T).GetProperty("Key");
                var translationsProperty = typeof(T).GetProperty("Translations");

                keyProperty?.SetValue(item, values[0]);
                translationsProperty?.SetValue(item, translations);

                results.Add(item);
            }

            return results;
        }

        public async Task SaveCultureAsync(Culture culture)
        {
            var cultureDir = Path.Combine(_basePath, culture.Name);
            Directory.CreateDirectory(cultureDir);

            // 保存城市名称
            var cityNamesPath = Path.Combine(cultureDir, "CityNames.csv");
            await SaveTranslationFileAsync(cityNamesPath, culture.CityNames);

            // 保存政治体系
            var politicalSystemsPath = Path.Combine(cultureDir, "PoliticalSystems.json");
            var politicalSystems = new PoliticalSystemsFile { PoliticalSystems = culture.PoliticalSystems };
            await File.WriteAllTextAsync(politicalSystemsPath,
                JsonSerializer.Serialize(politicalSystems, _jsonOptions));

            // 保存州名称
            var stateNamesPath = Path.Combine(cultureDir, "StateNames.csv");
            await SaveTranslationFileAsync(stateNamesPath, culture.StateNames);

            // 更新内存中的数据
            _cultures[culture.Name] = culture;
        }

        private async Task SaveTranslationFileAsync<T>(string filePath, IEnumerable<T> items)
        {
            var lines = new List<string>();

            // 获取所有可能的语言
            var languages = new HashSet<string>();
            foreach (var item in items)
            {
                var translations = item.GetType().GetProperty("Translations")?.GetValue(item) as Dictionary<string, string>;
                if (translations != null)
                {
                    foreach (var lang in translations.Keys)
                    {
                        languages.Add(lang);
                    }
                }
            }

            // 写入表头
            lines.Add($"Key,{string.Join(",", languages)}");

            // 写入数据行
            foreach (var item in items)
            {
                var key = item.GetType().GetProperty("Key")?.GetValue(item)?.ToString() ?? "";
                var translations = item.GetType().GetProperty("Translations")?.GetValue(item) as Dictionary<string, string>;
                var values = languages.Select(lang => translations?.GetValueOrDefault(lang, "")).ToList();
                lines.Add($"{key},{string.Join(",", values)}");
            }

            await File.WriteAllLinesAsync(filePath, lines);
        }

        public Culture GetCulture(string name)
        {
            return _cultures.TryGetValue(name, out var culture) ? culture : null;
        }
    }

    internal class PoliticalSystemsFile
    {
        public List<string> PoliticalSystems { get; set; }
    }
}