using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Models
{
    public class ModUploadModel
    {
        public ModItemModel ModItem { get; set; }
        public string ModFilePath { get; set; }
        public string ModPreviewImagePath { get; set; }
        public bool IsUpdateModFile { get; set; }
        public bool IsUpdatePreviewImage { get; set; }
        public bool IsUpdateModInfo { get; set; }
    }
}