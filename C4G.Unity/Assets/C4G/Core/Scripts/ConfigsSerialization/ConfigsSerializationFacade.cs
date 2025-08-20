using System.Collections.Generic;
using System.Linq;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using Newtonsoft.Json;

namespace C4G.Core
{
    public class ConfigsSerializationFacade
    {
        public Result<string, EC4GError> Serialize(ParsedSheet parsedSheet)
        {
            EC4GError error = ValidateParsedSheet(parsedSheet);
            if (error != EC4GError.None)
                return Result<string, EC4GError>.FromError(error);

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

            return Result<string, EC4GError>.FromValue(json);
        }

        private static EC4GError ValidateParsedSheet(ParsedSheet parsedSheet)
        {
            if (string.IsNullOrEmpty(parsedSheet.Name))
                return EC4GError.CS_ParsedSheetNameNullOrEmpty;

            if (parsedSheet.Properties == null)
                return EC4GError.CS_ParsedSheetPropertiesNull;

            if (parsedSheet.Entities == null)
                return EC4GError.CS_ParsedSheetEntitiesNull;

            HashSet<string> propertyNamesHashSet = new HashSet<string>();
            foreach (ParsedPropertyInfo parsedPropertyInfo in parsedSheet.Properties)
            {
                if (!propertyNamesHashSet.Add(parsedPropertyInfo.Name))
                    return EC4GError.CS_ParsedSheetDuplicatedPropertyName;
            }
            
            foreach (IReadOnlyCollection<string> entity in parsedSheet.Entities)
            {
                if (entity.Count != parsedSheet.Properties.Count)
                    return EC4GError.CS_ParsedSheetMismatchedEntitiesCount;
            }

            return EC4GError.None;
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