using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using FMSModManager.Core.Services;
using FMSModManager.Pages.Layout;
using System.IO;
using Prism.Events;

namespace FMSModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            
            // 获取示例文件路径
            var examplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Example");
            
            // 注册服务
            services.AddWpfBlazorWebView();
            services.AddMudServices();
            services.AddSingleton<ReligionService>(new ReligionService(examplePath));
            services.AddSingleton<CultureService>(new CultureService(examplePath));
            services.AddSingleton<LocalizationService>(new LocalizationService(examplePath));
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddSingleton<LanguageService>(new LanguageService(AppDomain.CurrentDomain.BaseDirectory));


            Resources.Add("services", services.BuildServiceProvider());
        }
    }
}