using System;
using System.Collections.Generic;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsingFacade
    {
        public Result<C4GSheetsParsingError> ParseSheetToList(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase, List<ParsedConfig> parsedConfigs)
        {
            if (!ValidateParameters(sheetName, sheetData, parserBase, parsedConfigs, out string error))
                return Result<C4GSheetsParsingError>.FromError(new C4GSheetsParsingError(error, null));

            Result<C4GSheetsParsingError> result = parserBase.ParseToList(sheetName, sheetData, parsedConfigs);
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
                    string subType = property.Name[..dotIndex];
                    string subName = property.Name[(dotIndex + 1)..];
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
                error = "Sheet name must be not null or empty";
                return false;
            }

            if (parserBase == null)
            {
                error = $"'{sheetName}'. Parser must be provided";
                return false;
            }

            if (sheetData == null)
            {
                error = $"'{sheetName}'. Sheet data must be not null";
                return false;
            }

            if (parsedConfigs == null)
            {
                error = $" '{sheetName}'. Parsed configs list must not be null";
                return false;
            }

            return true;
        }
    }
}