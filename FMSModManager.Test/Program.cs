using FMSModManager.Core.Services;

namespace FMSModManager.Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            string modPath = @"E:\Project\CSharp\FantasyMapSimulator\FMSModManager\FMSModManager\bin\Debug\net8.0-windows\Example";
            var modMgr = new CultureModService(modPath);

            var mod = new CultureModModel(@"E:\Project\CSharp\FantasyMapSimulator\FMSModManager\FMSModManager\bin\Debug\net8.0-windows\Example\Culture\EasternFantasy");

            //mod.LoadCultureMod();
            Console.ReadLine();
        }
    }
}
