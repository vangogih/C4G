using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using Newtonsoft.Json;
using Entity = System.Collections.Generic.IReadOnlyDictionary<string, object>;
using EntitiesList = System.Collections.Generic.IReadOnlyList<System.Collections.Generic.IReadOnlyDictionary<string, object>>;

namespace C4G.Core
{
    public sealed class ConfigsSerialization
    {
        public Result<EntitiesList, string> ParseToEntitiesList(ParsedSheet parsedSheet)
        {
            bool isValid = ValidateParsedSheet(parsedSheet, out string error);
            if (!isValid)
                return Result<EntitiesList, string>.FromError(error);

            var entities = new List<Entity>(parsedSheet.Entities.Count);

            foreach (IReadOnlyCollection<string> entityData in parsedSheet.Entities)
            {
                Result<Entity, string> entityDataDictResult = GetEntityDataDict(entityData, parsedSheet.Properties);
                if (!entityDataDictResult.IsOk)
                    return Result<EntitiesList, string>.FromError(entityDataDictResult.Error);
                entities.Add(entityDataDictResult.Value);
            }

            return Result<EntitiesList, string>.FromValue(entities);
        }

        public Result<string, string> SerializeMultipleSheetsAsJsonObject(List<ParsedSheet> parsedSheets)
        {
            if (parsedSheets == null)
                return Result<string, string>.FromError("Parsed sheets cannot be null");

            var result = new Dictionary<string, EntitiesList>(parsedSheets.Count);

            foreach (ParsedSheet parsedSheet in parsedSheets)
            {
                Result<EntitiesList, string> sheetSerializationResult = ParseToEntitiesList(parsedSheet);
                if (!sheetSerializationResult.IsOk)
                    return Result<string, string>.FromError(sheetSerializationResult.Error);

                if (result.ContainsKey(parsedSheet.Name))
                    return Result<string, string>.FromError($"Duplicate sheet name '{parsedSheet.Name}'");

                result.Add(parsedSheet.Name, sheetSerializationResult.Value);
            }

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);

            return Result<string, string>.FromValue(json);
        }

        private static Result<Entity, string> GetEntityDataDict(
            IReadOnlyCollection<string> entityData, IReadOnlyList<ParsedPropertyInfo> properties)
        {
            var entityDataDict = new Dictionary<string, object>();

            int index = 0;

            foreach (ParsedPropertyInfo property in properties)
            {
                string serializedPropertyValue = entityData.ElementAt(index);
                Result<object, string> propertyValueResult = GetPropertyValue(property, serializedPropertyValue);
                if (!propertyValueResult.IsOk)
                    return Result<Entity, string>.FromError(propertyValueResult.Error);
                    
                entityDataDict[property.Name] = propertyValueResult.Value;
                index++;
            }

            return Result<Entity, string>.FromValue(entityDataDict);
        }

        private static readonly Dictionary<string, Func<string, Result<object, string>>> SimpleTypeParsers
            = new Dictionary<string, Func<string, Result<object, string>>>
        {
            { "int", TryParseFuncResultDecorator<int>(int.TryParse) },
            { "float", TryParseFuncResultDecorator((string s, out float f) => float.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out f)) },
            { "double", TryParseFuncResultDecorator((string s, out double d) => double.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) },
            { "bool", TryParseFuncResultDecorator<bool>(bool.TryParse) },
            { "string", Result<object, string>.FromValue }
        };

        private static readonly List<(Regex typePattern, char separator)> CollectionParsers = new List<(Regex typePattern, char separator)> 
        {
            (new Regex("^List<(.+)>$", RegexOptions.Compiled | RegexOptions.IgnoreCase), ',') 
        };

        private delegate bool TryParseFunc<T>(string serializedValue, out T value);

        private static Func<string, Result<object, string>> TryParseFuncResultDecorator<T>(TryParseFunc<T> tryParseFunc)
        {
            return serializedValue => tryParseFunc(serializedValue, out T value)
                ? Result<object, string>.FromValue(value)
                : Result<object, string>.FromError($"Could not parse property '{serializedValue}' as {typeof(T).Name}");
        }

        private static Result<object, string> GetPropertyValue(ParsedPropertyInfo property, string serializedPropertyValue)
        {
            if (SimpleTypeParsers.TryGetValue(property.Type, out Func<string, Result<object, string>> simpleTypeParser))
            {
                return simpleTypeParser(serializedPropertyValue);
            }

            foreach ((Regex typePattern, char separator) in CollectionParsers)
            {
                Match match = typePattern.Match(property.Type);

                if (match.Success)
                {
                    if (match.Length != property.Type.Length)
                        return Result<object, string>.FromError($"Collection parser regex pattern '{typePattern}' matches only part '{match.Value}' of property type '{property.Type}', but should only match whole type");

                    if (match.Groups.Count != 2)
                        return Result<object, string>.FromError($"Collection parser regex pattern '{typePattern}' captures '{match.Groups.Count}' groups but should capture only 2 - Collection pattern itself and collection type");

                    string elementType = match.Groups[1].Value;

                    return ParseList(serializedPropertyValue, elementType, separator);
                }
            }

            return Result<object, string>.FromError($"Cannot parse property with type '{property.Type}'");
        }

        private static Result<object, string> ParseList(string serializedList, string elementType, char separator)
        {
            if (string.IsNullOrWhiteSpace(serializedList))
                return Result<object, string>.FromValue(new List<object>());

            if (!SimpleTypeParsers.TryGetValue(elementType, out Func<string, Result<object, string>> simpleTypeParser))
                return Result<object, string>.FromError($"Cannot parse list elements type '{elementType}'");

            var result = new List<object>();
            string[] serializedElements = serializedList.Split(separator);

            foreach (string serializedElement in serializedElements)
            {
                string trimmedSerializedElement = serializedElement.Trim();
                if (string.IsNullOrEmpty(trimmedSerializedElement))
                    return Result<object, string>.FromError($"Cannot parse empty element in list '{serializedList}' with type '{elementType}'");

                Result<object, string> elementParseResult = simpleTypeParser(trimmedSerializedElement);

                if (!elementParseResult.IsOk)
                {
                    return Result<object, string>.FromError($"Cannot parse collection element '{serializedElement}' as {elementType}\n" +
                                                            $"Inner error: {elementParseResult.Error}");
                }

                result.Add(elementParseResult.Value);
            }

            return Result<object, string>.FromValue(result);
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
    }
}