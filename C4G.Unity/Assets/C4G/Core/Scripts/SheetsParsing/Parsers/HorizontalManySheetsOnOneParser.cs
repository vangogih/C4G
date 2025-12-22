using System;
using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [Serializable]
    public sealed class HorizontalManySheetsOnOneParser : SheetParserBase
    {
        public struct SheetOnSheet
        {
            public int StartRowIndex;
            public int StartColumnIndex;
            public string Name;
            public int EndRowIndex;
            public int EndColumnIndex;
        }

        public override Result<string> ParseNonAlloc(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            const string logTag = "HorizontalManySheetsOnOneParser. ParseNonAlloc";

            var sheetStartsList = new List<SheetOnSheet>();
            var sheetsList = new List<SheetOnSheet>();

            for (int rowIndex = 0; rowIndex < sheetData.Count; rowIndex++)
            {
                IList<object> row = sheetData[rowIndex];

                if (row == null)
                    return Result<string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{rowIndex + 1}' is null but shouldn't");

                for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                {
                    object cell = row[columnIndex];
                    string cellText = cell == null ? string.Empty : cell.ToString().Trim();
                    string[] cellTextSplitByPoint = cellText.Split('.');

                    if (cellTextSplitByPoint.Length != 2)
                        continue;

                    if (cellTextSplitByPoint[0] == "start")
                    {
                        string name = cellTextSplitByPoint[1];
                        bool nameValid = IsSheetOnSheetNameValid(name);
                        if (!nameValid)
                            return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has invalid name");

                        var sheetOnSheet = new SheetOnSheet
                        {
                            StartRowIndex = rowIndex,
                            StartColumnIndex = columnIndex,
                            Name = name
                        };
                        sheetStartsList.Add(sheetOnSheet);
                    }
                    else if (cellTextSplitByPoint[0] == "end")
                    {
                        string name = cellTextSplitByPoint[1];
                        bool nameValid = IsSheetOnSheetNameValid(name);
                        if (!nameValid)
                            return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has invalid name");

                        int matches = 0;
                        for (int sheetIndex = sheetStartsList.Count - 1; sheetIndex >= 0; --sheetIndex)
                        {
                            SheetOnSheet sheetOnSheet = sheetStartsList[sheetIndex];
                            if (rowIndex >= sheetOnSheet.StartRowIndex && columnIndex >= sheetOnSheet.StartColumnIndex)
                            {
                                if (++matches > 1)
                                    return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end with more than one matching starts");

                                if (!name.Equals(sheetOnSheet.Name, StringComparison.Ordinal))
                                    return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end with geometrically matching start cell [{sheetOnSheet.StartRowIndex + 1}][{sheetOnSheet.StartColumnIndex + 1}] but with different name '{sheetOnSheet.Name}'");

                                sheetOnSheet.EndRowIndex = rowIndex;
                                sheetOnSheet.EndColumnIndex = columnIndex;
                                sheetStartsList.RemoveAt(sheetIndex);
                                sheetsList.Add(sheetOnSheet);
                                break;
                            }
                        }

                        if (matches == 0)
                            return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end but there are no matching starts");
                    }
                }
            }

            if (sheetStartsList.Count > 0)
                return Result<string>.FromError($"{logTag}. There are no matching ends for {sheetStartsList.Count} starts");

            for (int sheetIndex = 0; sheetIndex < sheetsList.Count; sheetIndex++)
            {
                SheetOnSheet sheetOnSheet = sheetsList[sheetIndex];

                var parseHorizontalResult = SheetsParsingUtils.ParseHorizontal(
                    sheetOnSheet.Name,
                    sheetData,
                    startRowIndex: sheetOnSheet.StartRowIndex + 1,
                    startColumnIndex: sheetOnSheet.StartColumnIndex + 1,
                    endRowIndex: sheetOnSheet.EndRowIndex - 1,
                    endColumnIndex: sheetOnSheet.EndColumnIndex - 1);

                if (!parseHorizontalResult.IsOk)
                    return Result<string>.FromError(parseHorizontalResult.Error);

                parsedConfigs.Add(parseHorizontalResult.Value);
            }

            return Result<string>.Ok;
        }

        private static bool IsSheetOnSheetNameValid(string sheetName) => !string.IsNullOrEmpty(sheetName);
    }
}