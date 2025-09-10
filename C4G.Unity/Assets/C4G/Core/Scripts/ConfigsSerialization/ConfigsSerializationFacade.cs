using System.Collections.Generic;
using System.Linq;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using Newtonsoft.Json;

namespace C4G.Core
{
    public class ConfigsSerializationFacade
    {
        public Result<string, string> Serialize(ParsedSheet parsedSheet)
        {
            bool isValid = ValidateParsedSheet(parsedSheet, out string error);
            if (!isValid)
                return Result<string, string>.FromError(error);

            var jsonDto = new JsonDto(parsedSheet.Name, new List<Dictionary<string, string>>());

            foreach (IReadOnlyCollection<string> entityData in parsedSheet.Entities)
            {
                var entityDataDict = new Dictionary<string, string>();

                int index = 0;
                foreach (var property in parsedSheet.Properties)
                {
                    entityDataDict[property.Name] = entityData.ElementAt(index);
                    index++;
                }

                jsonDto.Entities.Add(entityDataDict);
            }

            string json = JsonConvert.SerializeObject(jsonDto, Formatting.Indented);

            return Result<string, string>.FromValue(json);
        }

        private static bool ValidateParsedSheet(ParsedSheet parsedSheet, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(parsedSheet.Name))
                error = "Configs serialization error. ParsedSheet name is null or empty";
            else if (parsedSheet.Properties == null)
                error = "Configs serialization error. ParsedSheet properties are null";
            else if (parsedSheet.Entities == null)
                error = "Configs serialization error. ParsedSheet entities are null";
            else
            {
                HashSet<string> propertyNamesHashSet = new HashSet<string>();
                foreach (ParsedPropertyInfo parsedPropertyInfo in parsedSheet.Properties)
                {
                    if (!propertyNamesHashSet.Add(parsedPropertyInfo.Name))
                    {
                        error = "Configs serialization error. ParsedSheet has duplicated property names";
                        break;
                    }
                }

                if (string.IsNullOrEmpty(error))
                {
                    foreach (IReadOnlyCollection<string> entity in parsedSheet.Entities)
                    {
                        if (entity.Count != parsedSheet.Properties.Count)
                        {
                            error = "Configs serialization error. Entity count doesn't match properties count";
                            break;
                        }
                    }
                }
            }

            return string.IsNullOrEmpty(error);
        }

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
    }
}