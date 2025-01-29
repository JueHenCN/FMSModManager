using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FMSModManager.Core.Services
{
    public class ReligionService
    {
        private readonly string _examplePath;

        public ReligionService(string examplePath)
        {
            _examplePath = examplePath;
        }

        /// <summary>
        /// Reads and parses a Religion.json file for a given mod name.
        /// </summary>
        /// <param name="modName">The name of the mod (e.g., "Atheism").</param>
        /// <returns>A dictionary containing the religion data.</returns>
        public Dictionary<string, object> ReadReligion(string modName)
        {
            var filePath = Path.Combine(_examplePath, "Religion", modName, "Religion.json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Religion.json not found for mod: {modName}");
            }

            var json = File.ReadAllText(filePath);
            var religionData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return religionData;
        }

        /// <summary>
        /// Writes updated religion data to a Religion.json file.
        /// </summary>
        /// <param name="modName">The name of the mod (e.g., "Atheism").</param>
        /// <param name="data">The updated religion data.</param>
        public void WriteReligion(string modName, Dictionary<string, object> data)
        {
            var filePath = Path.Combine(_examplePath, "Religion", modName, "Religion.json");
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
