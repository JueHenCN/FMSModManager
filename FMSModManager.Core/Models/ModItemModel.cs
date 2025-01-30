using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Models
{
    public class ModItemModel
    {
        public ulong ModId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateData { get; set; }
        public string UpdateData { get; set; }
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = new();

    }
}
