using System;
using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [Serializable]
    public sealed class ManyConfigsOnOneSheetParser : SheetParserBase
    {
        public struct ConfigFrame
        {
            public int StartRowIndex;
            public int StartColumnIndex;
            public string Name;
            public int EndRowIndex;
            public int EndColumnIndex;
        }

        public override Result<string> ParseNonAlloc(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs)
        {
            const string logTag = "ManyConfigsOnOneSheetParser. ParseNonAlloc";

            var configFrameStarts = new List<ConfigFrame>();
            var configFrames = new List<ConfigFrame>();

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
                        bool nameValid = IsConfigFrameNameValid(name);
                        if (!nameValid)
                            return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has invalid name");

                        var configFrame = new ConfigFrame
                        {
                            StartRowIndex = rowIndex,
                            StartColumnIndex = columnIndex,
                            Name = name
                        };
                        configFrameStarts.Add(configFrame);
                    }
                    else if (cellTextSplitByPoint[0] == "end")
                    {
                        string name = cellTextSplitByPoint[1];
                        bool nameValid = IsConfigFrameNameValid(name);
                        if (!nameValid)
                            return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has invalid name");

                        int matches = 0;
                        for (int configFrameIndex = configFrameStarts.Count - 1; configFrameIndex >= 0; --configFrameIndex)
                        {
                            ConfigFrame configFrame = configFrameStarts[configFrameIndex];
                            if (rowIndex >= configFrame.StartRowIndex && columnIndex >= configFrame.StartColumnIndex)
                            {
                                if (++matches > 1)
                                    return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end with more than one matching starts");

                                if (!name.Equals(configFrame.Name, StringComparison.Ordinal))
                                    return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end with geometrically matching start cell [{configFrame.StartRowIndex + 1}][{configFrame.StartColumnIndex + 1}] but with different name '{configFrame.Name}'");

                                configFrame.EndRowIndex = rowIndex;
                                configFrame.EndColumnIndex = columnIndex;
                                configFrameStarts.RemoveAt(configFrameIndex);
                                configFrames.Add(configFrame);
                                break;
                            }
                        }

                        if (matches == 0)
                            return Result<string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end but there are no matching starts");
                    }
                }
            }

            if (configFrameStarts.Count > 0)
                return Result<string>.FromError($"{logTag}. There are no matching ends for {configFrameStarts.Count} starts");

            for (int configIndex = 0; configIndex < configFrames.Count; configIndex++)
            {
                ConfigFrame configFrame = configFrames[configIndex];

                int innerStartRow = configFrame.StartRowIndex + 1;
                int innerStartCol = configFrame.StartColumnIndex + 1;
                int innerEndRow = configFrame.EndRowIndex - 1;
                int innerEndCol = configFrame.EndColumnIndex - 1;

                var parseResult = ParseConfigFrame(logTag, configFrame, sheetData, innerStartRow, innerStartCol, innerEndRow, innerEndCol);
                if (!parseResult.IsOk)
                    return Result<string>.FromError(parseResult.Error);

                parsedConfigs.Add(parseResult.Value);
            }

            return Result<string>.Ok;
        }

        private static Result<ParsedConfig, string> ParseConfigFrame(
            string logTag,
            ConfigFrame configFrame,
            IList<IList<object>> sheetData,
            int innerStartRow, int innerStartCol,
            int innerEndRow, int innerEndCol)
        {
            int innerHeight = innerEndRow - innerStartRow + 1;
            int innerWidth = innerEndCol - innerStartCol + 1;

            // Vertical format requires at least 2 inner rows (names row + types row)
            bool canBeVertical = innerHeight >= 2;
            // Horizontal format requires at least 2 inner columns (names col + types col)
            bool canBeHorizontal = innerWidth >= 2;

            if (!canBeVertical && !canBeHorizontal)
            {
                return Result<ParsedConfig, string>.FromError(
                    $"{logTag}. Config frame '{configFrame.Name}' at [{configFrame.StartRowIndex + 1}][{configFrame.StartColumnIndex + 1}] " +
                    $"has inner area too small for any format (inner height: {innerHeight}, inner width: {innerWidth}). " +
                    $"Vertical requires at least 2 rows, horizontal requires at least 2 columns");
            }

            if (canBeVertical && !canBeHorizontal)
            {
                return SheetsParsingUtils.ParseVertical(
                    configFrame.Name, sheetData,
                    startRowIndex: innerStartRow, startColumnIndex: innerStartCol,
                    endRowIndex: innerEndRow, endColumnIndex: innerEndCol);
            }

            if (canBeHorizontal && !canBeVertical)
            {
                return SheetsParsingUtils.ParseHorizontal(
                    configFrame.Name, sheetData,
                    startRowIndex: innerStartRow, startColumnIndex: innerStartCol,
                    endRowIndex: innerEndRow, endColumnIndex: innerEndCol);
            }

            // Both formats are geometrically possible — try both, use whichever succeeds
            var verticalResult = SheetsParsingUtils.ParseVertical(
                configFrame.Name, sheetData,
                startRowIndex: innerStartRow, startColumnIndex: innerStartCol,
                endRowIndex: innerEndRow, endColumnIndex: innerEndCol);

            var horizontalResult = SheetsParsingUtils.ParseHorizontal(
                configFrame.Name, sheetData,
                startRowIndex: innerStartRow, startColumnIndex: innerStartCol,
                endRowIndex: innerEndRow, endColumnIndex: innerEndCol);

            if (verticalResult.IsOk && horizontalResult.IsOk)
            {
                return Result<ParsedConfig, string>.FromError(
                    $"{logTag}. Config frame '{configFrame.Name}' at [{configFrame.StartRowIndex + 1}][{configFrame.StartColumnIndex + 1}] " +
                    $"is ambiguous: both vertical and horizontal parsing succeeded. " +
                    $"Use dedicated HorizontalManyConfigsOnOneSheetParser or VerticalManyConfigsOnOneSheetParser instead");
            }

            if (verticalResult.IsOk)
                return verticalResult;

            if (horizontalResult.IsOk)
                return horizontalResult;

            return Result<ParsedConfig, string>.FromError(
                $"{logTag}. Config frame '{configFrame.Name}' at [{configFrame.StartRowIndex + 1}][{configFrame.StartColumnIndex + 1}] " +
                $"could not be parsed as either format. " +
                $"Vertical error: {verticalResult.Error}. " +
                $"Horizontal error: {horizontalResult.Error}");
        }

        private static bool IsConfigFrameNameValid(string sheetName) => !string.IsNullOrEmpty(sheetName);
    }
}
