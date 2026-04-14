using System.Collections.Generic;
using C4G.Core.Utils;
using C4G.Unity.Assets.C4G.Core.Scripts.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public sealed class VerticalSheetParser : SheetParserBase
    {
        public override Result<C4GError> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            if (sheetData.Count < 3)
                return Result<C4GError>.FromError(new C4GError.SheetsParsing($"Rows amount '{sheetData.Count}' < 3", sheetName));

            if (sheetData[0].Count < 1)
                return Result<C4GError>.FromError(new C4GError.SheetsParsing($"Columns amount '{sheetData[0].Count}' < 1", sheetName));

            int dataRowLength = sheetData[0].Count;

            var parseVerticalResult = SheetsParsingUtils.ParseVertical(
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