using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        public static readonly string TypeHeader = "C4G_TYPE";
        public static readonly string NameHeader = "C4G_NAME";

        public Result<ParsedSheet, string> ParseSheet(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase)
        {
            if (!ValidateParameters(sheetName, sheetData, parserBase, out string error))
                return Result<ParsedSheet, string>.FromError(error);

            return parserBase.Parse(sheetName, sheetData);
        }

        private bool ValidateParameters(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(sheetName))
            {
                error = "Sheet name is null or empty";
                return false;
            }

            else if (string.IsNullOrEmpty(sheetName))
                error = "Sheets parsing error. Sheet name must be not null or empty";
            else if (sheetData == null)
                error = "Sheets parsing error. Sheet data must be not null";
            else if (sheetData.Count < 2)
                error = "Sheets parsing error. Sheet data length must be equal or greater than two";
            else if (parserBase == null)
                error = "Sheets parsing error. Parser must be provided";
            else
            {
                parserBase.Validate(sheetData);
            }

            return string.IsNullOrEmpty(error);
        }
    }
}