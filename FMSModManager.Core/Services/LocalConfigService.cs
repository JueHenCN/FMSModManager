using FMSModManager.Core.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services
{
    public class LocalConfigService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly SteamworkService _steamworkService;
        private const string CONFIG_PATH = "config/localConfig.json";
        private LocalConfigModel localConfigModel;
        public LocalConfigService(SteamworkService steamwork, IEventAggregator eventAggregator)
        { 
            _eventAggregator = eventAggregator;
            _steamworkService = steamwork;
            LoadLocalConfig();
        }

        private void LoadLocalConfig()
        {
            //localConfigModel = 
        }
    }

    public class LocalConfigModel
    {
        public string LocalGameFolder;

        public string SelLanguageName;
    }
}
