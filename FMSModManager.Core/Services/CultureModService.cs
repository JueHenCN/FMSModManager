using CsvHelper;
using CsvHelper.Configuration;
using FMSModManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services
{
    public class CultureModService
    {
        private readonly string _gamePath;
        public List<CultureModModel> CultureMods { get; private set; } = new List<CultureModModel>();

        public CultureModService(string gamePath)
        {
            _gamePath = Path.Combine(gamePath, "Culture");
            LoadCultureMods();
        }

        private void LoadCultureMods()
        {
            var modDirectories = Directory.GetDirectories(_gamePath);
            CultureMods = modDirectories.Select(dir => new CultureModModel(dir)).ToList();
        }

    }

    public class CultureModModel
    {
        public readonly string ModName;
        private readonly string _gameRootPath;

        public List<ModTextModel> StateNames { get; private set; } = new List<ModTextModel>();
        public List<ModTextModel> CityNames { get; private set; } = new List<ModTextModel>();
        public List<string> PoliticalSystems { get; private set; } = new List<string>();

        // ==================== 构造函数与初始化 ====================
        public CultureModModel(string gameRootPath)
        {
            ModName = Path.GetFileName(Path.GetDirectoryName(gameRootPath))!;
            _gameRootPath = gameRootPath;
            LoadAllData();
        }

        private void LoadAllData()
        {
            StateNames = LoadCsvData(FileName.StateNames);
            CityNames = LoadCsvData(FileName.CityNames);
            PoliticalSystems = LoadJsonData(FileName.PoliticalSystems);
        }

        // ==================== 文件路径管理 ====================
        private static class FileName
        {
            public const string StateNames = "StateNames.csv";
            public const string CityNames = "CityNames.csv";
            public const string PoliticalSystems = "PoliticalSystems.json";
        }

        private string GetFilePath(string fileName) => Path.Combine(_gameRootPath, fileName);

        // ==================== 数据读取 ====================
        private List<ModTextModel> LoadCsvData(string fileName)
        {
            var filePath = GetFilePath(fileName);
            EnsureFileInitialized(filePath, InitializeCsvFile<ModTextModel>);

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, GetCsvConfig());
            return csv.GetRecords<ModTextModel>()
                     .Where(r => !string.IsNullOrEmpty(r.Key))
                     .ToList();
        }

        private List<string> LoadJsonData(string fileName)
        {
            var filePath = GetFilePath(fileName);
            EnsureFileInitialized(filePath, InitializeJsonFile);

            var jsonString = File.ReadAllText(filePath, Encoding.UTF8);
            return JsonSerializer.Deserialize<PoliticalSystemConfig>(jsonString)?.PoliticalSystems
                   ?? new List<string>();
        }

        // ==================== 数据保存 ====================
        public void SaveStateNames() => SaveCsvData(FileName.StateNames, StateNames);
        public void SaveCityNames() => SaveCsvData(FileName.CityNames, CityNames);

        public void SavePoliticalSystems()
        {
            var config = new PoliticalSystemConfig { PoliticalSystems = PoliticalSystems };
            SaveJsonData(FileName.PoliticalSystems, config);
        }

        // ==================== 核心工具方法 ====================
        #region 文件操作
        private void EnsureFileInitialized(string filePath, Action<string> initializeAction)
        {
            EnsureDirectoryExists(filePath);
            if (!File.Exists(filePath)) initializeAction(filePath);
        }

        private void EnsureDirectoryExists(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void InitializeCsvFile<T>(string filePath)
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteHeader<T>();
            csv.NextRecord();
        }

        private void InitializeJsonFile(string filePath)
        {
            var initialData = new PoliticalSystemConfig
            {
                PoliticalSystems = new List<string> { "Kingdom", "Empire" }
            };
            File.WriteAllText(filePath,
                            JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true }),
                            Encoding.UTF8);
        }
        #endregion

        #region 数据序列化
        private CsvConfiguration GetCsvConfig() => new(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLower(),
            IgnoreBlankLines = true,
            Encoding = Encoding.UTF8
        };

        private void SaveCsvData<T>(string fileName, IEnumerable<T> data)
        {
            var filePath = GetFilePath(fileName);
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, GetCsvConfig());
            csv.WriteRecords(data);
        }

        private void SaveJsonData<T>(string fileName, T data)
        {
            var filePath = GetFilePath(fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(data, options), Encoding.UTF8);
        }
        #endregion

        private class PoliticalSystemConfig
        {
            public List<string> PoliticalSystems { get; set; } = new List<string>();
        }
    }
}

