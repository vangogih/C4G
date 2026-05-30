using System.Collections.Generic;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public sealed class HorizontalSheetParser : SheetParserBase
    {
        public override Result<C4GSheetsParsingError> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            if (sheetData.Count < 1)
                return Result<C4GSheetsParsingError>.FromError(new C4GSheetsParsingError($"Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < 1", null));

            int dataRowLength = sheetData[0].Count;

            Result<ParsedConfig, C4GSheetsParsingError> parseHorizontalResult = SheetsParsingUtils.ParseHorizontal(
                sheetName,
                sheetData,
                startRowIndex: 0,
                startColumnIndex: 0,
                endRowIndex: sheetData.Count - 1,
                endColumnIndex: dataRowLength - 1);

            if (!parseHorizontalResult.IsOk)
                return parseHorizontalResult.WithoutValue();

            parsedConfigs.Add(parseHorizontalResult.Value);

            return Result<C4GSheetsParsingError>.Ok;
        }
    }
}