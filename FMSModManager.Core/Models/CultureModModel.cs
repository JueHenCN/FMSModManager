using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Models
{
    public class CultureModModel
    {
        public List<TextEntity> StateNames { get; set; } = new List<TextEntity>();
        public List<TextEntity> CityNames { get; set; } = new List<TextEntity>();
        public List<string> PoliticalSystems { get; set; } = new List<string>();
    }
}
