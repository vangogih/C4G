using System;
using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [Serializable]
    public sealed class HorizontalManyConfigsOnOneSheetParser : SheetParserBase
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
            const string logTag = "HorizontalManySheetsOnOneParser. ParseNonAlloc";

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

            for (int sheetIndex = 0; sheetIndex < configFrames.Count; sheetIndex++)
            {
                ConfigFrame configFrame = configFrames[sheetIndex];

                var parseHorizontalResult = SheetsParsingUtils.ParseHorizontal(
                    configFrame.Name,
                    sheetData,
                    startRowIndex: configFrame.StartRowIndex + 1,
                    startColumnIndex: configFrame.StartColumnIndex + 1,
                    endRowIndex: configFrame.EndRowIndex - 1,
                    endColumnIndex: configFrame.EndColumnIndex - 1);

                if (!parseHorizontalResult.IsOk)
                    return Result<string>.FromError(parseHorizontalResult.Error);

                parsedConfigs.Add(parseHorizontalResult.Value);
            }

            return Result<string>.Ok;
        }

        private static bool IsConfigFrameNameValid(string sheetName) => !string.IsNullOrEmpty(sheetName);
    }
}