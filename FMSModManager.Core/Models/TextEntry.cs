using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace FMSModManager.Core.Models
{
    public class TextEntry
    {
        [Name("Key")]
        public string Key { get; set; } = string.Empty;

        [Name("Chinese")]
        public string Chinese { get; set; } = string.Empty;

        [Name("English")]
        public string English { get; set; } = string.Empty;

        [Name("TraditionalChinese")]
        public string TraditionalChinese { get; set; } = string.Empty;

        [Name("Russian")]
        public string Russian { get; set; } = string.Empty;

        [Name("SpanishSpain")]
        public string SpanishSpain { get; set; } = string.Empty;

        [Name("PortugueseBrazil")]
        public string PortugueseBrazil { get; set; } = string.Empty;

        [Name("German")]
        public string German { get; set; } = string.Empty;

        [Name("Japanese")]
        public string Japanese { get; set; } = string.Empty;

        [Name("French")]
        public string French { get; set; } = string.Empty;

        [Name("Polish")]
        public string Polish { get; set; } = string.Empty;

        [Name("SpanishLatinAmerica")]
        public string SpanishLatinAmerica { get; set; } = string.Empty;

        [Name("PortuguesePortugal")]
        public string PortuguesePortugal { get; set; } = string.Empty;

        [Name("Korean")]
        public string Korean { get; set; } = string.Empty;
    }
}
