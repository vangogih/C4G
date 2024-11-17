using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace C4G.Editor
{
    public sealed class ParsedSheetToJsonConverter
    {
        public static void ConvertParsedSheetToJson(ParsedSheet parsedSheet, string fileName)
        {
            try
            {
                var jsonOutput = new
                {
                    parsedSheet.Name,
                    Entities = new List<Dictionary<string, object>>()
                };

                foreach (var entity in parsedSheet.Entities)
                {
                    var entityDict = new Dictionary<string, object>();

                    int index = 0;
                    foreach (var property in parsedSheet.Properties)
                    {
                        entityDict[property.Name] = entity.ElementAtOrDefault(index);
                        index++;
                    }

                    jsonOutput.Entities.Add(entityDict);
                }

                var jsonString = JsonConvert.SerializeObject(jsonOutput, Formatting.Indented);

                string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets");

                // Ensure the StreamingAssets directory exists
                if (!Directory.Exists(streamingAssetsPath))
                {
                    Directory.CreateDirectory(streamingAssetsPath);
                }

                // Get the full path to the output JSON file
                string filePath = Path.Combine(streamingAssetsPath, $"{fileName}.json");

                // Write the JSON string to the file
                File.WriteAllText(filePath, jsonString);

                Debug.Log($"JSON file was successfully created at {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create JSON file: {ex.Message}");
            }
        }
    }
}