using FMSModManager.Core.Events;
using FMSModManager.Core.Services.Interface;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services
{
    public class LocalConfigService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SteamworkService _steamworkService;
        private readonly IFileService _fileService;
        private const string CONFIG_PATH = "Config/localConfig.json";
        public LocalConfigModel LocalConfig;

        public LocalConfigService(SteamworkService steamwork, IFileService fileService, IEventAggregator eventAggregator)

        { 
            _eventAggregator = eventAggregator;
            _steamworkService = steamwork;
            _fileService = fileService;
            eventAggregator.GetEvent<LanguageChangedEvent>().Subscribe(OnLanguageChanged);
            eventAggregator.GetEvent<GameFolderChangedEvent>().Subscribe(OnGameFolderChanged);
            LoadLocalConfig();
        }

        private void OnLanguageChanged(string language)
        {
            LocalConfig.SelLanguageName = language;
            _fileService.WriteJson(CONFIG_PATH, LocalConfig);
        }

        private void OnGameFolderChanged(string gameFolder)
        {
            LocalConfig.LocalGameFolder = gameFolder;
            _fileService.WriteJson(CONFIG_PATH, LocalConfig);
        }

        private void LoadLocalConfig()
        {
            if (File.Exists(CONFIG_PATH))
            {
                LocalConfig = _fileService.ReadJson<LocalConfigModel>(CONFIG_PATH);
            }
            else
            {
                LocalConfig = new LocalConfigModel();
            }

            if (string.IsNullOrEmpty(LocalConfig.LocalGameFolder))
            {
                LocalConfig.LocalGameFolder = _steamworkService.GetGameFolder();
                _fileService.WriteJson(CONFIG_PATH, LocalConfig);
            }
        }
    }



    public class LocalConfigModel
    {
        public string LocalGameFolder { get; set; } = "";

        public string SelLanguageName { get; set; } = "简体中文";
    }
}
