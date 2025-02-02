using System.Text;

namespace FMSModManager.Core.Services
{
    public class LocalizationService
    {
        private readonly string _examplePath;

        public LocalizationService(string examplePath)
        {
            _examplePath = examplePath;
        }

        public Dictionary<string, Dictionary<string, string>> ReadLocalization(string modName, string type)
        {
            var filePath = Path.Combine(_examplePath, type, modName, "localization.csv");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"未找到本地化文件: {filePath}");
            }

            var result = new Dictionary<string, Dictionary<string, string>>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            
            if (lines.Length == 0)
            {
                return result;
            }

            // 读取表头（语言列表）
            var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();
            
            // 处理每一行数据
            for (int i = 1; i < lines.Length; i++)
            {
                var values = SplitCSVLine(lines[i]);
                if (values.Length != headers.Length) continue;

                var key = values[0].Trim();
                var translations = new Dictionary<string, string>();
                
                for (int j = 1; j < headers.Length; j++)
                {
                    translations[headers[j]] = values[j].Trim();
                }

                result[key] = translations;
            }

            return result;
        }

        private string[] SplitCSVLine(string line)
        {
            var result = new List<string>();
            var inQuotes = false;
            var currentValue = new StringBuilder();
            
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                
                if (line[i] == ',' && !inQuotes)
                {
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                    continue;
                }
                
                currentValue.Append(line[i]);
            }
            
            result.Add(currentValue.ToString());
            return result.ToArray();
        }

        public void WriteLocalization(string modName, string type, Dictionary<string, Dictionary<string, string>> data)
        {
            var filePath = Path.Combine(_examplePath, type, modName, "localization.csv");
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var languages = data.Values.FirstOrDefault()?.Keys.ToList() ?? new List<string>();
            var sb = new StringBuilder();

            // 写入表头
            sb.AppendLine($"Key,{string.Join(",", languages)}");

            // 写入数据
            foreach (var entry in data)
            {
                var translations = languages.Select(lang => 
                {
                    var value = entry.Value.GetValueOrDefault(lang, "");
                    if (value.Contains(",") || value.Contains("\""))
                    {
                        return $"\"{value.Replace("\"", "\"\"")}\"";
                    }
                    return value;
                });
                sb.AppendLine($"{entry.Key},{string.Join(",", translations)}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public Dictionary<string, string> GetLanguages()
        {
            return new Dictionary<string, string>
            {
                { "Chinese", "简体中文" },
                { "TraditionalChinese", "繁體中文" },
                { "English", "English" },
                { "Russian", "Русский" },
                { "SpanishSpain", "Español (España)" },
                { "PortugueseBrazil", "Português (Brasil)" },
                { "German", "Deutsch" },
                { "Japanese", "日本語" },
                { "French", "Français" },
                { "Polish", "Polski" },
                { "SpanishLatinAmerica", "Español (Latinoamérica)" },
                { "PortuguesePortugal", "Português (Portugal)" },
                { "Korean", "한국어" },
                { "Turkish", "Türkçe" }
            };
        }
    }
} 