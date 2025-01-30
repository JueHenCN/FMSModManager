using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Models
{
    public class Culture
    {
        public string Name { get; set; }
        public List<CityName> CityNames { get; set; } = new();
        public List<string> PoliticalSystems { get; set; } = new();
        public List<StateName> StateNames { get; set; } = new();
    }

    public class CityName
    {
        public string Key { get; set; }
        public Dictionary<string, string> Translations { get; set; } = new();
    }

    public class StateName
    {
        public string Key { get; set; }
        public Dictionary<string, string> Translations { get; set; } = new();
    }
}
