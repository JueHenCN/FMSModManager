using CsvHelper.Configuration;
using CsvHelper;
using FMSModManager.Core.Services.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FMSModManager.Core.Services
{
    public sealed class FileService : IFileService
    {
        private readonly Encoding _defaultEncoding;
        private readonly CsvConfiguration _defaultCsvConfig;
        private readonly JsonSerializerOptions _defaultJsonOptions;

        // 通过构造函数注入配置参数
        public FileService(
            Encoding? defaultEncoding = null,
            CsvConfiguration? defaultCsvConfig = null,
            JsonSerializerOptions? defaultJsonOptions = null)
        {
            _defaultEncoding = defaultEncoding ?? Encoding.UTF8;
            _defaultCsvConfig = defaultCsvConfig ?? new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLower(),
                IgnoreBlankLines = true,
                Encoding = _defaultEncoding
            };
            _defaultJsonOptions = defaultJsonOptions ?? new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
            };
        }

        // ==================== 基础文件操作 ====================
        public void EnsureDirectoryExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void EnsureFileInitialized(string filePath, string defaultContent = "", Encoding? encoding = null)
        {
            EnsureDirectoryExists(filePath);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, defaultContent, encoding ?? _defaultEncoding);
            }
        }

        // ==================== CSV处理 ====================
        public List<T> ReadCsv<T>(string filePath, CsvConfiguration? config = null)
        {
            var finalConfig = config ?? _defaultCsvConfig;
            EnsureFileInitialized(filePath, InitializeCsvHeader<T>());

            using var reader = new StreamReader(filePath, finalConfig.Encoding);
            using var csv = new CsvReader(reader, finalConfig);
            return csv.GetRecords<T>().ToList();
        }

        public List<T> ReadCsvString<T>(string csvString, CsvConfiguration? config = null)
        {
            var finalConfig = config ?? _defaultCsvConfig;
            using var reader = new StringReader(csvString);
            using var csv = new CsvReader(reader, finalConfig);
            return csv.GetRecords<T>().ToList();
        }

        public void WriteCsv<T>(string filePath, IEnumerable<T> records, CsvConfiguration? config = null)
        {
            var finalConfig = config ?? _defaultCsvConfig;
            EnsureDirectoryExists(filePath);

            using var writer = new StreamWriter(filePath, false, finalConfig.Encoding);
            using var csv = new CsvWriter(writer, finalConfig);
            csv.WriteRecords(records);
        }

        public string ConvertCsvToString<T>(IEnumerable<T> records, CsvConfiguration? config = null)
        {
            var finalConfig = config ?? _defaultCsvConfig;
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, finalConfig);
            csv.WriteRecords(records);
            return writer.ToString();
        }
        private string InitializeCsvHeader<T>()
        {
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, _defaultCsvConfig);
            csv.WriteHeader<T>();
            csv.NextRecord();
            return writer.ToString();
        }

        // ==================== JSON处理 ====================
        public T ReadJson<T>(string filePath)
        {
            EnsureFileInitialized(filePath, "{}");
            var jsonString = File.ReadAllText(filePath, _defaultEncoding);
            return JsonSerializer.Deserialize<T>(jsonString, _defaultJsonOptions)!;
        }

        public void WriteJson<T>(string filePath, T data, JsonSerializerOptions? options = null)
        {
            var finalOptions = options ?? _defaultJsonOptions;
            EnsureDirectoryExists(filePath);
            File.WriteAllText(filePath,
                            JsonSerializer.Serialize(data, finalOptions),
                            _defaultEncoding);
        }

        // ==================== 文件夹处理 ====================
        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}

