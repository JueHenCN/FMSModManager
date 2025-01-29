using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace FMSModManager.Core.Services
{
    public class Religion
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string HolyBookName { get; set; }
        public List<string> Practices { get; set; }
    }

    public class ReligionFile
    {
        public List<Religion> Religions { get; set; }
    }

    public class ReligionService
    {
        private readonly string _examplePath;

        public ReligionService(string examplePath)
        {
            _examplePath = examplePath;
        }

        /// <summary>
        /// 读取指定mod的Religion.json文件
        /// </summary>
        /// <param name="modName">mod名称 (例如: "Atheism")</param>
        /// <returns>宗教数据</returns>
        public ReligionFile ReadReligion(string modName)
        {
            var filePath = Path.Combine(_examplePath, "Religion", modName, "Religion.json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"未找到宗教mod文件: {modName}");
            }

            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ReligionFile>(json, options);
        }

        /// <summary>
        /// 获取所有可用的宗教mod列表
        /// </summary>
        /// <returns>mod名称列表</returns>
        public List<string> GetAvailableReligionMods()
        {
            var religionPath = Path.Combine(_examplePath, "Religion");
            if (!Directory.Exists(religionPath))
            {
                return new List<string>();
            }

            return Directory.GetDirectories(religionPath)
                          .Select(Path.GetFileName)
                          .ToList();
        }

        /// <summary>
        /// 写入更新后的宗教数据到Religion.json文件
        /// </summary>
        /// <param name="modName">mod名称 (例如: "Atheism")</param>
        /// <param name="data">更新后的宗教数据</param>
        public void WriteReligion(string modName, ReligionFile data)
        {
            var filePath = Path.Combine(_examplePath, "Religion", modName, "Religion.json");
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        public void DeleteReligion(string modName)
        {
            var dirPath = Path.Combine(_examplePath, "Religion", modName);
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);
            }
            else
            {
                throw new DirectoryNotFoundException($"未找到宗教mod目录: {modName}");
            }
        }
    }
}
