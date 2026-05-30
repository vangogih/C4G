using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using C4G.Core.ConfigsSerialization.SimpleTypeParsers;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using Newtonsoft.Json;
using Entity = System.Collections.Generic.IReadOnlyDictionary<string, object>;
using EntitiesList = System.Collections.Generic.IReadOnlyList<System.Collections.Generic.IReadOnlyDictionary<string, object>>;
using C4G.Core.Errors;

namespace C4G.Core.ConfigsSerialization
{
    public sealed class ConfigsSerializer : IConfigsSerializer
    {
        private static readonly Dictionary<string, IC4GTypeParser> SimpleTypeParsers
            = new Dictionary<string, IC4GTypeParser>
            {
                { "int", new IntParser() },
                { "float", new FloatParser() },
                { "double", new DoubleParser() },
                { "bool", new BoolParser() },
                { "string", new StringParser() }
            };

        private static readonly List<(Regex typePattern, char separator)> DefaultCollectionParsers =
            new List<(Regex typePattern, char separator)>
            {
                (new Regex("^List<(.+)>$", RegexOptions.Compiled | RegexOptions.IgnoreCase), ',')
            };

        private readonly List<(Regex typePattern, char separator)> _collectionParsers;

        public ConfigsSerializer() : this(DefaultCollectionParsers) { }

        internal ConfigsSerializer(List<(Regex typePattern, char separator)> collectionParsers)
        {
            _collectionParsers = collectionParsers;
        }
        
        public Result<EntitiesList, C4GConfigsSerializationError> ParseToEntitiesList(
            ParsedConfig parsedConfig,
            IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            bool isValid = ValidateParsedConfig(parsedConfig, out string error);
            if (!isValid)
                return Result<EntitiesList, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(error, null));

            var entities = new List<Entity>(parsedConfig.Entities.Count);

            foreach (IReadOnlyCollection<string> entityData in parsedConfig.Entities)
            {
                Result<Entity, C4GConfigsSerializationError> entityDataDictResult = GetEntityDataDict(entityData, parsedConfig.Properties, parsedConfig.SubTypes, aliasParsersByName);
                if (!entityDataDictResult.IsOk)
                    return Result<EntitiesList, C4GConfigsSerializationError>.FromError(entityDataDictResult.Error);
                entities.Add(entityDataDictResult.Value);
            }

            return Result<EntitiesList, C4GConfigsSerializationError>.FromValue(entities);
        }

        public Result<string, C4GConfigsSerializationError> SerializeParsedConfigsAsJsonObject(
            List<ParsedConfig> parsedConfigs,
            IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            if (parsedConfigs == null)
                return Result<string, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError("Parsed configs cannot be null", null));

            var result = new Dictionary<string, EntitiesList>(parsedConfigs.Count);

            foreach (ParsedConfig parsedConfig in parsedConfigs)
            {
                Result<EntitiesList, C4GConfigsSerializationError> sheetSerializationResult = ParseToEntitiesList(parsedConfig, aliasParsersByName);
                if (!sheetSerializationResult.IsOk)
                    return Result<string, C4GConfigsSerializationError>.FromError(sheetSerializationResult.Error);

                if (result.ContainsKey(parsedConfig.Name))
                    return Result<string, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError($"Duplicate sheet name '{parsedConfig.Name}'", null));

                result.Add(parsedConfig.Name, sheetSerializationResult.Value);
            }

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return Result<string, C4GConfigsSerializationError>.FromValue(json);
        }

        private Result<Entity, C4GConfigsSerializationError> GetEntityDataDict(
            IReadOnlyCollection<string> entityData,
            IReadOnlyList<ParsedPropertyInfo> properties,
            List<string> subTypes,
            IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            var entityDataDict = new Dictionary<string, object>();

            int index = 0;

            foreach (ParsedPropertyInfo property in properties)
            {
                if (property.SubTypeIndex < 0)
                {
                    string serializedPropertyValue = entityData.ElementAt(index);
                    Result<object, C4GConfigsSerializationError> propertyValueResult = GetPropertyValue(property, serializedPropertyValue, aliasParsersByName);
                    if (!propertyValueResult.IsOk)
                        return Result<Entity, C4GConfigsSerializationError>.FromError(propertyValueResult.Error);

                    entityDataDict[property.Name] = propertyValueResult.Value;
                }
                index++;
            }
            for (int i = 0; i < subTypes.Count; i++)
            {
                string subType = subTypes[i];
                var subTypeDataDict = new Dictionary<string, object>();
                index = 0;
                foreach (ParsedPropertyInfo property in properties)
                {
                    if (property.SubTypeIndex == i)
                    {
                        string serializedPropertyValue = entityData.ElementAt(index);
                        Result<object, C4GConfigsSerializationError> propertyValueResult = GetPropertyValue(property, serializedPropertyValue, aliasParsersByName);
                        if (!propertyValueResult.IsOk)
                            return Result<Entity, C4GConfigsSerializationError>.FromError(propertyValueResult.Error);

                        subTypeDataDict[property.Name] = propertyValueResult.Value;
                    }
                    index++;
                }
                entityDataDict[$"{subType}_Instance"] = subTypeDataDict;
            }

            return Result<Entity, C4GConfigsSerializationError>.FromValue(entityDataDict);
        }

        private Result<object, C4GConfigsSerializationError> GetPropertyValue(
            ParsedPropertyInfo property,
            string serializedPropertyValue,
            IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            if (SimpleTypeParsers.TryGetValue(property.Type, out IC4GTypeParser simpleTypeParser))
            {
                return simpleTypeParser.Parse(serializedPropertyValue);
            }

            foreach ((Regex typePattern, char separator) in _collectionParsers)
            {
                Match match = typePattern.Match(property.Type);

                if (match.Success)
                {
                    if (match.Length != property.Type.Length)
                        return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(
                            $"Collection parser regex pattern '{typePattern}' matches only part '{match.Value}' of property type '{property.Type}', but should only match whole type", null));

                    if (match.Groups.Count != 2)
                        return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(
                            $"Collection parser regex pattern '{typePattern}' captures '{match.Groups.Count}' groups but should capture only 2 - Collection pattern itself and collection type", null));

                    string elementType = match.Groups[1].Value;

                    return ParseList(serializedPropertyValue, elementType, separator);
                }
            }

            if (aliasParsersByName.TryGetValue(property.Type, out IC4GTypeParser parser))
            {
                return parser.Parse(serializedPropertyValue);
            }

            return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError($"Cannot parse property with type '{property.Type}'", null));
        }

        private static Result<object, C4GConfigsSerializationError> ParseList(string serializedList, string elementType, char separator)
        {
            if (string.IsNullOrWhiteSpace(serializedList))
                return Result<object, C4GConfigsSerializationError>.FromValue(new List<object>());

            if (!SimpleTypeParsers.TryGetValue(elementType, out IC4GTypeParser simpleTypeParser))
                return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError($"Cannot parse list elements type '{elementType}'", null));

            var result = new List<object>();
            string[] serializedElements = serializedList.Split(separator);

            foreach (string serializedElement in serializedElements)
            {
                string trimmedSerializedElement = serializedElement.Trim();
                if (string.IsNullOrEmpty(trimmedSerializedElement))
                    return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(
                        $"Cannot parse empty element in list '{serializedList}' with type '{elementType}'", null));

                Result<object, C4GConfigsSerializationError> elementParseResult = simpleTypeParser.Parse(trimmedSerializedElement);

                if (!elementParseResult.IsOk)
                {
                    return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(
                        $"Cannot parse collection element '{serializedElement}' as {elementType}\n" +
                        $"Inner error: {elementParseResult.Error}", null));
                }

                result.Add(elementParseResult.Value);
            }

            return Result<object, C4GConfigsSerializationError>.FromValue(result);
        }

        private static bool ValidateParsedConfig(ParsedConfig parsedConfig, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(parsedConfig.Name))
                error = "ParsedConfig name is null or empty";
            else if (parsedConfig.Properties == null)
                error = "ParsedConfig properties are null";
            else if (parsedConfig.Entities == null)
                error = "ParsedConfig entities are null";
            else
            {
                HashSet<string> propertyNamesHashSet = new HashSet<string>();
                foreach (ParsedPropertyInfo parsedPropertyInfo in parsedConfig.Properties)
                {
                    if (parsedPropertyInfo.SubTypeIndex < 0 && !propertyNamesHashSet.Add(parsedPropertyInfo.Name))
                    {
                        error = "ParsedConfig has duplicated property names";
                        break;
                    }
                }

                if (string.IsNullOrEmpty(error))
                {
                    for (int i = 0; i < parsedConfig.SubTypes.Count; i++)
                    {
                        propertyNamesHashSet.Clear();
                        foreach (ParsedPropertyInfo parsedPropertyInfo in parsedConfig.Properties)
                        {
                            if (parsedPropertyInfo.SubTypeIndex == i && !propertyNamesHashSet.Add(parsedPropertyInfo.Name))
                            {
                                error = "ParsedConfig has duplicated property names";
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(error))
                {
                    foreach (IReadOnlyCollection<string> entity in parsedConfig.Entities)
                    {
                        if (entity.Count != parsedConfig.Properties.Length)
                        {
                            error = "Entity count doesn't match properties count";
                            break;
                        }
                    }
                }
            }

            return string.IsNullOrEmpty(error);
        }
    }
}