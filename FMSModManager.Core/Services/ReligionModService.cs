using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMSModManager.Core.Models;
using FMSModManager.Core.Services.Interface;

namespace FMSModManager.Core.Services
{
    public class ReligionModService : IReligionModService
    {
        private readonly string _modsPath;
        private readonly IFileService _fileService;
        private readonly SteamworkService _steamworkService;
        private Dictionary<string, ReligionModModel?> _religionMods = new();

        public ReligionModService(IFileService fileService, SteamworkService steamworkService)
        {
            _fileService = fileService;
            _steamworkService = steamworkService;
            _modsPath = Path.Combine(_steamworkService.GetGameFolder(), "FantasyMapSimulator_Data\\StreamingAssets\\Base\\Religion");
            LoadReligionMods();
        }

        private void LoadReligionMods()
        {
            var modDirectories = Directory.GetDirectories(_modsPath);
            foreach (var dir in modDirectories)
                _religionMods.Add(Path.GetFileName(dir), null);
        }

        public List<string> GetAvailableReligionMods()
        {
            return _religionMods.Keys.ToList();
        }

        private bool VerifyModFileExists(string modName)
        {
            var _modPath = Path.Combine(_modsPath, modName);
            return Directory.Exists(_modPath) &&
                   File.Exists(Path.Combine(_modPath, ReligionFileNames.ReligionKey)) &&
                   File.Exists(Path.Combine(_modPath, ReligionFileNames.ReligionData));

        }

        public ReligionModModel? GetReligionMod(string modName, bool isRefresh = false)
        {
            if (string.IsNullOrEmpty(modName) ||
                !_religionMods.ContainsKey(modName) ||
                !VerifyModFileExists(modName))
                return null;
            if (_religionMods[modName] == null || isRefresh)
            {
                var mod = new ReligionModModel();
                mod.ReligionData = _fileService.ReadCsv<TextEntity>(Path.Combine(_modsPath, modName, ReligionFileNames.ReligionData));
                mod.Religions = _fileService.ReadJson<ReligionKeys>(Path.Combine(_modsPath, modName, ReligionFileNames.ReligionKey)).Religions;
                _religionMods[modName] = mod;

            }
            return _religionMods[modName];
        }

        public ReligionModModel CreateReligionMod(string modName)
        {
            var mod = new ReligionModModel(){
                Religions = new List<ReligionKey>(){
                    new ReligionKey(){
                        Name = $"{modName}NewReligionName",
                        Description = $"{modName}NewReligionDescription",
                        HolyBookName = $"{modName}NewHolyBookName",
                        Practices = new List<string> { $"{modName}NewPractice1", $"{modName}NewPractice2" }
                    }
                },
                ReligionData = new List<TextEntity>() 
                { 
                    new TextEntity() { Key = $"{modName}NewReligionDataKey", Chinese = $"{modName}NewReligionDataChinese", English = $"{modName}NewReligionDataEnglish" },
                    new TextEntity() { Key = $"{modName}NewReligionDescription" , Chinese = $"{modName}NewReligionDescriptionChinese", English = $"{modName}NewReligionDescriptionEnglish" },
                    new TextEntity() { Key = $"{modName}NewHolyBookName", Chinese = $"{modName}NewHolyBookNameChinese", English = $"{modName}NewHolyBookNameEnglish" },   
                    new TextEntity() { Key = $"{modName}NewPractice1", Chinese = $"{modName}NewPractice1Chinese", English = $"{modName}NewPractice1English" },
                    new TextEntity() { Key = $"{modName}NewPractice2", Chinese = $"{modName}NewPractice2Chinese", English = $"{modName}NewPractice2English" }
                }
            };
            _religionMods.Add(modName, mod);
            SaveReligionMod(modName);
            return mod;
        }

        public bool DeleteReligionMod(string modName)
        {
            try
            {
                if (!_religionMods.ContainsKey(modName))
                    return false;
                _fileService.DeleteDirectory(Path.Combine(_modsPath, modName));
                _religionMods.Remove(modName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateReligionMod(string modName, ReligionModModel mod)
        {
            if (null == mod || !_religionMods.ContainsKey(modName))
                return false;
            _religionMods[modName] = mod;
            return SaveReligionMod(modName);
        }

        public bool SaveReligionMod(string modName)
        {
            try
            {
                var mod = _religionMods[modName];
                _fileService.WriteCsv(Path.Combine(_modsPath, modName, ReligionFileNames.ReligionData), mod.ReligionData);
                _fileService.WriteJson(Path.Combine(_modsPath, modName, ReligionFileNames.ReligionKey), new ReligionKeys(){
                    Religions = mod.Religions
                });
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        private class ReligionKeys
        {
            public List<ReligionKey> Religions { get; set; } = new();
        }
    }

    public static class ReligionFileNames
    {
        public const string ReligionData = "localization.csv";
        public const string ReligionKey = "Religion.json";
    }

    public class ReligionModModel
    {
        public List<TextEntity> ReligionData { get; set; } = new();

        public List<ReligionKey> Religions { get; set; } = new();
        
    }

    public class ReligionKey
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string HolyBookName { get; set; }
        public List<string> Practices { get; set; }
    }
}
