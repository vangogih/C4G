using System.Collections.Generic;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public sealed class VerticalSheetParser : SheetParserBase
    {
        public override Result<C4GSheetsParsingError> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            if (sheetData.Count < 3)
                return Result<C4GSheetsParsingError>.FromError(new C4GSheetsParsingError($"Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < 3", null));

            if (sheetData[0].Count < 1)
                return Result<C4GSheetsParsingError>.FromError(new C4GSheetsParsingError($"Sheet name '{sheetName}'. Columns amount '{sheetData[0].Count}' < 1", null));

            int dataRowLength = sheetData[0].Count;

            Result<ParsedConfig, C4GSheetsParsingError> parseVerticalResult = SheetsParsingUtils.ParseVertical(
                sheetName,
                sheetData,
                startRowIndex: 0,
                startColumnIndex: 0,
                endRowIndex: sheetData.Count - 1,
                endColumnIndex: dataRowLength - 1);

            if (!parseVerticalResult.IsOk)
                return parseVerticalResult.WithoutValue();

            parsedConfigs.Add(parseVerticalResult.Value);

            return Result<C4GSheetsParsingError>.Ok;
        }
    }
}