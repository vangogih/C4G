using System.Collections.Generic;
using C4G.Core.SheetsParsing._0_RawParsing;
using C4G.Core.SheetsParsing._1_PropertiesHierarchyTraversal;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        public Result<string> ParseSheetNonAlloc(string sheetName, IList<IList<object>> sheetData, RawSheetParserBase rawParser, List<ParsedConfig> parsedConfigs)
        {
            if (!ValidateParameters(sheetName, sheetData, rawParser, parsedConfigs, out string error))
                return Result<string>.FromError(error);

            var rawParsedConfigs = new List<RawParsedConfig>();
            var rawParseResult = rawParser.ParseNonAlloc(sheetName, sheetData, rawParsedConfigs);
            if (!rawParseResult.IsOk)
                return rawParseResult;

            for (int configIndex = 0; configIndex < rawParsedConfigs.Count; configIndex++)
            {
                RawParsedConfig rawConfig = rawParsedConfigs[configIndex];

                var properties = new ParsedProperty[rawConfig.Properties.Count];
                for (int propertyIndex = 0; propertyIndex < rawConfig.Properties.Count; propertyIndex++)
                {
                    RawParsedProperty rawProperty = rawConfig.Properties[propertyIndex];
                    var property = new ParsedProperty(rawProperty.NameWithPossibleHierarchy, rawProperty.Type);
                    properties[propertyIndex] = property;
                }

                ParsedConfig config = new ParsedConfig(rawConfig.Name, properties, rawConfig.Entities);
                parsedConfigs.Add(config);
            }

            return Result<string>.Ok;
        }

        private static bool ValidateParameters(
            string sheetName,
            IList<IList<object>> sheetData,
            RawSheetParserBase parserBase,
            List<ParsedConfig> parsedConfigs,
            out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(sheetName))
            {
                error = "Sheets parsing error. Sheet name must be not null or empty";
                return false;
            }

            if (parserBase == null)
            {
                error = $"Sheets parsing error '{sheetName}'. Parser must be provided";
                return false;
            }

            if (sheetData == null)
            {
                error = $"Sheets parsing error '{sheetName}'. Sheet data must be not null";
                return false;
            }

            if (parsedConfigs == null)
            {
                error = $"Sheets parsing error '{sheetName}'. Parsed configs list must not be null";
                return false;
            }

            return true;
        }
    }
}