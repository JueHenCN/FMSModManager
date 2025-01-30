using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services
{
    public class LogService
    {
        public static void InitLogConfig()
        {
            // 获取程序所在的目录
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string dir = Path.GetDirectoryName(exePath);

            // 构建日志文件的绝对路径
            string logFilePath = Path.Combine(dir, "Logs", "Logger.log");

            Log.Logger = new LoggerConfiguration() // 创建日志配置
                .MinimumLevel.Debug() // 设置最低日志级别为 Debug
                .WriteTo.Console() // 写入控制台
                .WriteTo.Async(a => a.File(logFilePath, // 写入文件
                    rollingInterval: RollingInterval.Day, // 按天滚动
                    fileSizeLimitBytes: 10_000_000, // 限制单个日志文件大小 10 MB
                    rollOnFileSizeLimit: true, // 滚动到新文件
                    retainedFileCountLimit: null, // 保留文件数
                    shared: true)) // 线程共享文件
                .CreateLogger();
        }

        public static void Debug(string message) => Log.Logger.Debug($"{message}");

        public static void Info(string message) => Log.Logger.Information($"{message}");

        public static void Warn(string message) => Log.Logger.Warning($"{message}");

        public static void Error(string message) => Log.Logger.Error($"{message}");

        public static void Error(string message, Exception exception) => Log.Logger.Error(exception, $"{message}");

        public static void Fatal(string message) => Log.Logger.Fatal($"{message}");

        public static void Fatal(string message, Exception exception) => Log.Logger.Fatal(exception, $"{message}");
    }
}
