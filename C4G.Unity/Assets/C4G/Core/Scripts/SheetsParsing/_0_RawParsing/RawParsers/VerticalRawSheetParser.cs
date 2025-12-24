using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing._0_RawParsing.RawParsers
{
    [System.Serializable]
    public sealed class VerticalRawSheetParser : RawSheetParserBase
    {
        public override Result<string> ParseNonAlloc(string sheetName, IList<IList<object>> sheetData, List<RawParsedConfig> parsedConfigs)
        {
            if (sheetData.Count < 3)
                return Result<string>.FromError($"C4G Error. Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < 3");

            if (sheetData[0].Count < 1)
                return Result<string>.FromError($"C4G Error. Sheet name '{sheetName}'. Columns amount '{sheetData[0].Count}' < 1");

            int dataRowLength = sheetData[0].Count;

            var parseVerticalResult = RawParsingUtils.RawParseVertical(
                sheetName,
                sheetData,
                startRowIndex: 0,
                startColumnIndex: 0,
                endRowIndex: sheetData.Count - 1,
                endColumnIndex: dataRowLength - 1);

            if (!parseVerticalResult.IsOk)
                return Result<string>.FromError(parseVerticalResult.Error);

            parsedConfigs.Add(parseVerticalResult.Value);

            return Result<string>.Ok;
        }
    }
}