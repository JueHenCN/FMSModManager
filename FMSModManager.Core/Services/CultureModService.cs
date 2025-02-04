using CsvHelper;
using CsvHelper.Configuration;
using FMSModManager.Core.Models;
using FMSModManager.Core.Services.Interface;
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
    public class CultureModService : ICultureModService
    {
        private readonly string _modsPath;
        private readonly IFileService _fileService;
        private readonly SteamworkService _steamworkService;
        public List<CultureModModel> CultureMods { get; private set; } = new List<CultureModModel>();
        private Dictionary<string, CultureModModel?> _cultureMods = new();

        public CultureModService(SteamworkService steamworkService, IFileService fileService)
        {
            _fileService = fileService;
            _steamworkService = steamworkService;
            _modsPath = Path.Combine(_steamworkService.GetGameFolder(), "FantasyMapSimulator_Data\\StreamingAssets\\Base\\Culture");
            LoadCultureMods();
        }

        private void LoadCultureMods()
        {
            var modDirectories = Directory.GetDirectories(_modsPath);
            foreach (var dir in modDirectories)
                _cultureMods.Add(Path.GetFileName(dir), null);
        }

        private bool VerifyModFileExists(string modName)
        {
            var _modPath = Path.Combine(_modsPath, modName);
            return Directory.Exists(_modPath) &&
                   File.Exists(Path.Combine(_modPath, FileName.StateNames)) &&
                   File.Exists(Path.Combine(_modPath, FileName.CityNames)) &&
                   File.Exists(Path.Combine(_modPath, FileName.PoliticalSystems));
        }

        public List<string> GetAvailableCultureMods()
        {
            return _cultureMods.Keys.ToList();
        }

        public CultureModModel CreateCultureMod(string modName)

        {
            var mod = new CultureModModel();
            mod.CityNames.Add(new TextEntry() { Key = "TestKey", Chinese = "新的城市", English = "New City" });
            mod.StateNames.Add(new TextEntry() { Key = "TestKey", Chinese = "新的国家", English = "New State" });
            mod.PoliticalSystems.Add("Kingdom");
            _cultureMods.Add(modName, mod);
            _fileService.WriteCsv(Path.Combine(_modsPath, modName, FileName.StateNames), mod.StateNames);
            _fileService.WriteCsv(Path.Combine(_modsPath, modName, FileName.CityNames), mod.CityNames);
            _fileService.WriteJson(Path.Combine(_modsPath, modName, FileName.PoliticalSystems), new PoliticalSystemConfig() { PoliticalSystems = mod.PoliticalSystems });
            return mod;

        }

        public bool DeleteCultureMod(string modName)
        {
            if (!_cultureMods.ContainsKey(modName))
                return false;
            _cultureMods.Remove(modName);
            return true;
        }

        public CultureModModel? GetCultureMod(string modName, bool isRefresh = false)
        {
            if (string.IsNullOrEmpty(modName) ||
                !_cultureMods.ContainsKey(modName) ||
                !VerifyModFileExists(modName))
                return null;

            if (_cultureMods[modName] == null)
            {
                var mod = new CultureModModel();
                mod.StateNames = _fileService.ReadCsv<TextEntry>(Path.Combine(_modsPath, modName, FileName.StateNames));
                mod.CityNames = _fileService.ReadCsv<TextEntry>(Path.Combine(_modsPath, modName, FileName.CityNames));
                mod.PoliticalSystems = _fileService.ReadJson<PoliticalSystemConfig>(Path.Combine(_modsPath, modName, FileName.PoliticalSystems)).PoliticalSystems;
                _cultureMods[modName] = mod;
            }

            return _cultureMods[modName];
        }

        public void SaveCultureMod(string modName)
        {
            var mod = _cultureMods[modName];
            mod.StateNames = _fileService.ReadCsv<TextEntry>(Path.Combine(_modsPath, modName, FileName.StateNames));
            mod.CityNames = _fileService.ReadCsv<TextEntry>(Path.Combine(_modsPath, modName, FileName.CityNames));
            mod.PoliticalSystems = _fileService.ReadJson<PoliticalSystemConfig>(Path.Combine(_modsPath, modName, FileName.PoliticalSystems)).PoliticalSystems;
        }

        public void SaveStateNames(string modName)
        {
            var mod = _cultureMods[modName];
            _fileService.WriteCsv(Path.Combine(_modsPath, FileName.StateNames), mod.StateNames);
        }

        public void SaveCityNames(string modName)
        {
            var mod = _cultureMods[modName];
            _fileService.WriteCsv(Path.Combine(_modsPath, FileName.CityNames), mod.CityNames);
        }

        public void SavePoliticalSystems(string modName)
        {
            var mod = _cultureMods[modName];
            _fileService.WriteJson(Path.Combine(_modsPath, FileName.PoliticalSystems), new PoliticalSystemConfig() { PoliticalSystems = mod.PoliticalSystems });
        }

        private static class FileName
        {
            public const string StateNames = "StateNames.csv";
            public const string CityNames = "CityNames.csv";
            public const string PoliticalSystems = "PoliticalSystems.json";
        }

        private class PoliticalSystemConfig
        {
            public List<string> PoliticalSystems { get; set; } = new List<string>();
        }
    }

    public class CultureModModel
    {
        public List<TextEntry> StateNames { get; set; } = new List<TextEntry>();
        public List<TextEntry> CityNames { get; set; } = new List<TextEntry>();
        public List<string> PoliticalSystems { get; set; } = new List<string>();
    }
}

