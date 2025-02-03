using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services.Interface
{
    public interface IFileService
    {
        // 基础文件操作
        void EnsureDirectoryExists(string path);
        void EnsureFileInitialized(string filePath, string defaultContent = "", Encoding? encoding = null);

        // CSV处理
        List<T> ReadCsv<T>(string filePath, CsvConfiguration? config = null);
        void WriteCsv<T>(string filePath, IEnumerable<T> records, CsvConfiguration? config = null);

        // JSON处理
        T ReadJson<T>(string filePath);
        void WriteJson<T>(string filePath, T data, JsonSerializerOptions? options = null);
    }
}
