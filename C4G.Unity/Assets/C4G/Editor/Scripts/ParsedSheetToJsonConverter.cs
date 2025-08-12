using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace C4G.Editor
{
    public static class ParsedSheetToJsonConverter
    {
        [JsonObject]
        private sealed class JsonDto
        {
            [JsonProperty("name")]
            internal string Name { get; }

            [JsonProperty("entities")]
            internal List<Dictionary<string, string>> Entities { get; }

            internal JsonDto(string name, List<Dictionary<string, string>> entities)
            {
                Name = name;
                Entities = entities;
            }

            public override string ToString()
            {
                return $"{{ Name = {Name}, Entities = {Entities} }}";
            }
        }

        public static string ConvertParsedSheetToJson(ParsedSheet parsedSheet)
        {
            ThrowIfParsedSheetIsInvalid(parsedSheet);

            var jsonDto = new JsonDto(parsedSheet.Name, new List<Dictionary<string, string>>());

            foreach (IReadOnlyCollection<string> entityData in parsedSheet.Entities)
            {
                var entityDataDict = new Dictionary<string, string>();

                int index = 0;
                foreach (var property in parsedSheet.Properties)
                {
                    entityDataDict[property.Name] = entityData.ElementAt(index);
                    if (entityDataDict[property.Name].Contains(","))
                    {
                        entityDataDict[property.Name] = entityDataDict[property.Name].Replace(",", ",");
                        entityDataDict[property.Name] = $"[{entityDataDict[property.Name]}]";
                    }
                    index++;
                }

                jsonDto.Entities.Add(entityDataDict);
            }

            var result = JsonConvert.SerializeObject(jsonDto, Formatting.Indented);
            return result.Replace("\"[", "[").Replace("]\"", "]");

        }

        private static void ThrowIfParsedSheetIsInvalid(ParsedSheet parsedSheet)
        {
            if (parsedSheet == null)
                throw new NullReferenceException($"Parameter '{nameof(parsedSheet)}' is null");

            if (string.IsNullOrEmpty(parsedSheet.Name))
                throw new NullReferenceException($"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Name)}' is null or empty");

            if (parsedSheet.Properties == null)
                throw new NullReferenceException($"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Properties)}' is null");

            if (parsedSheet.Entities == null)
                throw new NullReferenceException($"Property '{nameof(parsedSheet)}.{nameof(parsedSheet.Entities)}' is null");
        }
    }
}