using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        public Result<string> ParseSheetNonAlloc(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase, List<ParsedSheet> parsedSheets)
        {
            if (!ValidateParameters(sheetName, sheetData, parserBase, parsedSheets, out string error))
                return Result<string>.FromError(error);

            return parserBase.ParseNonAlloc(sheetName, sheetData, parsedSheets);
        }

        private static bool ValidateParameters(
            string sheetName,
            IList<IList<object>> sheetData,
            SheetParserBase parserBase,
            List<ParsedSheet> parsedSheets,
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

            if (parsedSheets == null)
            {
                error = $"Sheets parsing error '{sheetName}'. Parsed sheets list must not be null";
                return false;
            }

            return true;
        }
    }
}