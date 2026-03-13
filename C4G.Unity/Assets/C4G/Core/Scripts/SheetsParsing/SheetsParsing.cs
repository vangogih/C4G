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

            return parserBase.ParseToList(sheetName, sheetData, parsedConfigs);
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