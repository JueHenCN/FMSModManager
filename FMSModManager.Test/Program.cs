using System.Globalization;
using System.Text.Json;
using CsvHelper.Configuration;
using CsvHelper;
using FMSModManager.Core.Models;
using FMSModManager.Core.Services;
using Prism.Events;

namespace FMSModManager.Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            string modPath = @"E:\Project\CSharp\FantasyMapSimulator\FMSModManager\FMSModManager\bin\Debug\net8.0-windows\Example";
            //var modMgr = new CultureModService(modPath, new FileService());

            //var mod = modMgr.GetModData("EasternFantasy");

            //mod.LoadCultureMod();

            //var eventAggregator = new EventAggregator();
            //var fileService = new FileService();
            //var local = new LocalConfigService(new SteamworkService(eventAggregator), fileService, eventAggregator);
            //var lang = new LanguageService(local,fileService,eventAggregator);

            //var editValue1 = lang.GetText("Edit");
            //Console.WriteLine(editValue1);
            //lang.UpdateLanguage("简体中文");
            //var editValue2 = lang.GetText("Edit");
            //Console.WriteLine(editValue2);

            //TestJsonString();

            TestReligion();

            Console.WriteLine("执行完成");
            Console.ReadLine();
        }

        static void TestReligion()
        {
            var eventAggregator = new EventAggregator();
            var fileService = new FileService();
            var local = new LocalConfigService(new SteamworkService(eventAggregator), fileService, eventAggregator);
            var lang = new LanguageService(local,fileService,eventAggregator);
            var steam = new SteamworkService(eventAggregator);

            var modMgr = new ReligionModService(fileService, steam);

            foreach (var item in modMgr.GetAvailableReligionMods())
            {
                Console.WriteLine(item);
                var mod = modMgr.GetReligionMod(item);
            }

            modMgr.CreateReligionMod("Test");




        }

        static void TestJsonString()
        {

            string csvString = "Key1,value1,value2\rKey2,value3";
            using (var reader = new StringReader(csvString))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null, // 忽略缺失字段
                HeaderValidated = null    // 跳过标题验证
            }))
            {
                var list = csv.GetRecords<TextEntity>().ToList();
            }
        }
    }
}
