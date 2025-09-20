using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        public const string TYPE_HEADER = "C4G_TYPE";
        public const string NAME_HEADER = "C4G_NAME";

        public Result<ParsedSheet, string> ParseSheet(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase)
        {
            if (!ValidateParameters(sheetName, sheetData, parserBase, out string error))
                return Result<ParsedSheet, string>.FromError(error);

            return parserBase.Parse(sheetName, sheetData);
        }

        private static bool ValidateParameters(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase, out string error)
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

            string parserError = parserBase.Validate(sheetName, sheetData);
            if (!string.IsNullOrEmpty(parserError))
            {
                error = parserError;
                return false;
            }

            return true;
        }
    }
}