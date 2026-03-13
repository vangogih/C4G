using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public sealed class HorizontalSheetParser : SheetParserBase
    {
        public override Result<string> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            if (sheetData.Count < 1)
                return Result<string>.FromError($"C4G Error. Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < 1");

            int dataRowLength = sheetData[0].Count;

            var parseHorizontalResult = SheetsParsingUtils.ParseHorizontal(
                sheetName,
                sheetData,
                startRowIndex: 0,
                startColumnIndex: 0,
                endRowIndex: sheetData.Count - 1,
                endColumnIndex: dataRowLength - 1);

            if (!parseHorizontalResult.IsOk)
                return Result<string>.FromError(parseHorizontalResult.Error);

            parsedConfigs.Add(parseHorizontalResult.Value);

            return Result<string>.Ok;
        }
    }
}