using CsvHelper;
using CsvHelper.Configuration;
using FMSModManager.Core.Models;
using FMSModManager.Core.Services.Interface;
using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            mod.CityNames.Add(new TextEntity() { Key = "TestKey", Chinese = "新的城市", English = "New City" });
            mod.StateNames.Add(new TextEntity() { Key = "TestKey", Chinese = "新的国家", English = "New State" });
            mod.PoliticalSystems.Add("Kingdom");
            mod.PoliticalSystems.Add("Empire");
            _cultureMods.Add(modName, mod);
            _fileService.WriteCsv(Path.Combine(_modsPath, modName, FileName.StateNames), mod.StateNames);
            _fileService.WriteCsv(Path.Combine(_modsPath, modName, FileName.CityNames), mod.CityNames);
            _fileService.WriteJson(Path.Combine(_modsPath, modName, FileName.PoliticalSystems), new PoliticalSystemConfig() { PoliticalSystems = mod.PoliticalSystems });
            return mod;

        }

        public bool DeleteCultureMod(string modName)
        {
            try
            {
                if (!_cultureMods.ContainsKey(modName))
                    return false;
                _fileService.DeleteDirectory(Path.Combine(_modsPath, modName));
                _cultureMods.Remove(modName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool UpdateCultureMod(string modName, CultureModModel? cultureMod)
        {
            if (null == cultureMod || !_cultureMods.ContainsKey(modName))
                return false;
            _cultureMods[modName] = cultureMod;
            return SaveCultureMod(modName);
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
                mod.StateNames = _fileService.ReadCsv<TextEntity>(Path.Combine(_modsPath, modName, FileName.StateNames));
                mod.CityNames = _fileService.ReadCsv<TextEntity>(Path.Combine(_modsPath, modName, FileName.CityNames));
                mod.PoliticalSystems = _fileService.ReadJson<PoliticalSystemConfig>(Path.Combine(_modsPath, modName, FileName.PoliticalSystems)).PoliticalSystems;
                _cultureMods[modName] = mod;

            }

            return _cultureMods[modName];
        }

        public bool SaveCultureMod(string modName)
        {
            try
            {
                var mod = _cultureMods[modName];
                _fileService.WriteCsv(Path.Combine(_modsPath, modName, FileName.StateNames), mod.StateNames);
                _fileService.WriteCsv(Path.Combine(_modsPath, modName, FileName.CityNames), mod.CityNames);
                _fileService.WriteJson(Path.Combine(_modsPath, modName, FileName.PoliticalSystems), new PoliticalSystemConfig() { PoliticalSystems = mod.PoliticalSystems });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
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
        public List<TextEntity> StateNames { get; set; } = new List<TextEntity>();
        public List<TextEntity> CityNames { get; set; } = new List<TextEntity>();
        public List<string> PoliticalSystems { get; set; } = new List<string>();
    }
}

