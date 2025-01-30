using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Models
{
    public class ModItemModel
    {
        public ModItemModel() { }

        public ModItemModel(ulong modId, string title, string description, string createData, string updateData, bool isPublished, List<string> tags)
        {
            ModId = modId;
            Title = title;
            Description = description;
            CreateData = createData;
            UpdateData = updateData;
            IsPublished = isPublished;
            Tags = tags;
        }

        public ulong ModId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateData { get; set; }
        public string UpdateData { get; set; }
        public bool IsPublished { get; set; }
        public List<string> Tags { get; set; } = new();

    }
}
