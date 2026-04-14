using System.Collections.Generic;
using C4G.Core.Utils;
using C4G.Unity.Assets.C4G.Core.Scripts.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        public Result<C4GError> ParseSheetToList(string sheetName, IList<IList<object>> sheetData, SheetParserBase parserBase, List<ParsedConfig> parsedConfigs)
        {
            if (!ValidateParameters(sheetName, sheetData, parserBase, parsedConfigs, out C4GError error))
                return Result<C4GError>.FromError(error);

            return parserBase.ParseToList(sheetName, sheetData, parsedConfigs);
        }

        private static bool ValidateParameters(
            string sheetName,
            IList<IList<object>> sheetData,
            SheetParserBase parserBase,
            List<ParsedConfig> parsedConfigs,
            out C4GError error)
        {
            error = default;

            if (string.IsNullOrEmpty(sheetName))
            {
                error = new C4GError.SheetsParsing("Sheet name must be not null or empty");
                return false;
            }

            if (parserBase == null)
            {
                error = new C4GError.SheetsParsing($"Parser must be provided", sheetName);
                return false;
            }

            if (sheetData == null)
            {
                error = new C4GError.SheetsParsing($"Sheet data must be not null", sheetName);
                return false;
            }

            if (parsedConfigs == null)
            {
                error = new C4GError.SheetsParsing($"Parsed configs list must not be null", sheetName);
                return false;
            }

            return true;
        }
    }
}