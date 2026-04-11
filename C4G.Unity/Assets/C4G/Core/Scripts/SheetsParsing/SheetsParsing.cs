using System;
using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        public Result<string> ParseSheetToList(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase, List<ParsedConfig> parsedConfigs)
        {
            if (!ValidateParameters(sheetName, sheetData, parserBase, parsedConfigs, out string error))
                return Result<string>.FromError(error);

            Result<string> result = parserBase.ParseToList(sheetName, sheetData, parsedConfigs);
            if (!result.IsOk)
                return result;

            for (int i = 0; i < parsedConfigs.Count; i++)
            {
                ParsedConfig config = parsedConfigs[i];
                for (int j = 0; j < config.Properties.Length; j++)
                {
                    ref ParsedPropertyInfo property = ref config.Properties[j];
                    int dotIndex = property.Name.IndexOf('.');
                    if (dotIndex < 0)
                        continue;
                    string subType = property.Name.Substring(0, dotIndex);
                    string subName = property.Name.Substring(dotIndex + 1, property.Name.Length - dotIndex - 1);
                    int subTypeIndex = -1;
                    for (int k = 0; k < config.SubTypes.Count; k++)
                    {
                        string existingSubType = config.SubTypes[k];
                        if (string.Equals(subType, existingSubType, StringComparison.Ordinal))
                        {
                            subTypeIndex = k;
                            break;
                        }
                    }
                    if (subTypeIndex < 0)
                    {
                        config.SubTypes.Add(subType);
                        subTypeIndex = config.SubTypes.Count - 1;
                    }
                    property.Name = subName;
                    property.SubTypeIndex = subTypeIndex;
                }
            }
            return result;
        }

        private static bool ValidateParameters(
            string sheetName,
            IList<IList<object>> sheetData,
            SheetParserBase parserBase,
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