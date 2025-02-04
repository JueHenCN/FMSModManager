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

            var eventAggregator = new EventAggregator();
            var fileService = new FileService();
            var local = new LocalConfigService(new SteamworkService(eventAggregator), fileService, eventAggregator);
            var lang = new LanguageService(local,fileService,eventAggregator);

            var editValue1 = lang.GetText("Edit");
            Console.WriteLine(editValue1);
            lang.UpdateLanguage("简体中文");
            var editValue2 = lang.GetText("Edit");
            Console.WriteLine(editValue2);

            Console.ReadLine();
        }
    }
}
