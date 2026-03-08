using System;
using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    internal struct ConfigFrame
    {
        public int StartRowIndex;
        public int StartColumnIndex;
        public string Name;
        public int EndRowIndex;
        public int EndColumnIndex;
    }

    internal static class SheetsParsingUtils
    {
        internal static Result<ParsedConfig, string> ParseHorizontal(string sheetName, IList<IList<object>> sheetData, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            string validationError = ValidateIndicesHorizontal(sheetName, startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
            if (!string.IsNullOrEmpty(validationError))
                return Result<ParsedConfig, string>.FromError(validationError);

            if (sheetData.Count <= endRowIndex)
                return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < expected '{endRowIndex + 1}'");

            int propertiesAmount = endRowIndex - startRowIndex + 1;
            ParsedPropertyInfo[] properties = new ParsedPropertyInfo[propertiesAmount];

            int entitiesWithTypeDeclarationAmount = endColumnIndex - startColumnIndex + 1;
            int entitiesAmount = entitiesWithTypeDeclarationAmount - 2;
            List<List<string>> entities = new List<List<string>>(entitiesAmount);

            for (var i = 0; i < entitiesAmount; i++)
            {
                entities.Add(new List<string>(propertiesAmount));
            }

            for (int rowIndex = startRowIndex; rowIndex <= endRowIndex; rowIndex++)
            {
                IList<object> row = sheetData[rowIndex];

                if (row.Count <= endColumnIndex)
                    return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{rowIndex + 1}' length '{row.Count}' < expected '{endColumnIndex + 1}'");

                string propertyName = (string)row[startColumnIndex];
                if (string.IsNullOrEmpty(propertyName))
                    return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Cell [{rowIndex + 1}][{startColumnIndex + 1}] must contain property name, but has null or empty value instead");

                string propertyType = (string)row[startColumnIndex + 1];
                if (string.IsNullOrEmpty(propertyType))
                    return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Cell [{rowIndex + 1}][{startColumnIndex + 2}] must contain property type, but has null or empty value instead");

                var parsedPropertyInfo = new ParsedPropertyInfo(propertyName, propertyType);
                properties[rowIndex - startRowIndex] = parsedPropertyInfo;

                for (int columnIndex = startColumnIndex + 2; columnIndex <= endColumnIndex; columnIndex++)
                {
                    entities[columnIndex - startColumnIndex - 2].Add((string)row[columnIndex]);
                }
            }

            var parsedConfig = new ParsedConfig(sheetName, properties, entities);
            return Result<ParsedConfig, string>.FromValue(parsedConfig);
        }

        private static string ValidateIndicesHorizontal(string sheetName, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            if (startRowIndex < 0)
                return $"C4G Error. Sheet name '{sheetName}'. Start row index '{startRowIndex}' must be greater than or equal to 0";
            if (startRowIndex > endRowIndex)
                return $"C4G Error. Sheet name '{sheetName}'. Start row index '{startRowIndex}' must be lower than or equal to end row index '{endRowIndex}'";

            if (startColumnIndex < 0)
                return $"C4G Error. Sheet name '{sheetName}'. Start column index {startColumnIndex} must be greater than or equal to 0";
            if (endColumnIndex - startColumnIndex < 2)
                return $"C4G Error. Sheet name '{sheetName}'. End column index '{endColumnIndex}' minus start column index '{startColumnIndex}' must be greater than or equal to 2";

            return string.Empty;
        }

        internal static Result<ParsedConfig, string> ParseVertical(string sheetName, IList<IList<object>> sheetData, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            string validationError = ValidateIndicesVertical(sheetName, startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
            if (!string.IsNullOrEmpty(validationError))
                return Result<ParsedConfig, string>.FromError(validationError);

            if (sheetData.Count <= endRowIndex)
                return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < expected '{endRowIndex + 1}'");

            IList<object> namesRow = sheetData[startRowIndex];
            if (namesRow.Count <= endColumnIndex)
                return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{startRowIndex + 1}' length '{namesRow.Count}' < expected '{endColumnIndex + 1}'");

            IList<object> typesRow = sheetData[startRowIndex + 1];
            if (typesRow.Count <= endColumnIndex)
                return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{startRowIndex + 2}' length '{typesRow.Count}' < expected '{endColumnIndex + 1}'");

            int propertiesAmount = endColumnIndex - startColumnIndex + 1;
            ParsedPropertyInfo[] properties = new ParsedPropertyInfo[propertiesAmount];

            for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
            {
                string propertyName = (string)namesRow[columnIndex];
                if (string.IsNullOrEmpty(propertyName))
                    return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Cell [{startRowIndex + 1}][{columnIndex + 1}] must contain property name, but has null or empty value instead");

                string propertyType = (string)typesRow[columnIndex];
                if (string.IsNullOrEmpty(propertyType))
                    return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Cell [{startRowIndex + 2}][{columnIndex + 1}] must contain property type, but has null or empty value instead");

                var parsedPropertyInfo = new ParsedPropertyInfo(propertyName, propertyType);
                properties[columnIndex - startColumnIndex] = parsedPropertyInfo;
            }

            int entitiesWithTypeDeclarationAmount = endRowIndex - startRowIndex + 1;
            int entitiesAmount = entitiesWithTypeDeclarationAmount - 2;
            List<List<string>> entities = new List<List<string>>(entitiesAmount);

            for (int rowIndex = startRowIndex + 2; rowIndex <= endRowIndex; rowIndex++)
            {
                IList<object> row = sheetData[rowIndex];

                if (row.Count <= endColumnIndex)
                    return Result<ParsedConfig, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{rowIndex + 1}' length '{row.Count}' < expected '{endColumnIndex + 1}'");

                List<string> entityValues = new List<string>(propertiesAmount);
                for (int columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
                {
                    entityValues.Add((string)row[columnIndex]);
                }

                entities.Add(entityValues);
            }

            var parsedConfig = new ParsedConfig(sheetName, properties, entities);
            return Result<ParsedConfig, string>.FromValue(parsedConfig);
        }

        private static string ValidateIndicesVertical(string sheetName, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            if (startRowIndex < 0)
                return $"C4G Error. Sheet name '{sheetName}'. Start row index '{startRowIndex}' must be greater than or equal to 0";
            if (endRowIndex - startRowIndex < 2)
                return $"C4G Error. Sheet name '{sheetName}'. End row index '{endRowIndex}' minus start row index '{startRowIndex}' must be greater than or equal to 2";

            if (startColumnIndex < 0)
                return $"C4G Error. Sheet name '{sheetName}'. Start column index {startColumnIndex} must be greater than or equal to 0";
            if (startColumnIndex > endColumnIndex)
                return $"C4G Error. Sheet name '{sheetName}'. Start column index '{startColumnIndex}' must be lower than or equal to end column index '{endColumnIndex}'";

            return string.Empty;
        }

        internal static Result<List<ConfigFrame>, string> ParseConfigFrames(string sheetName, IList<IList<object>> sheetData)
        {
            const string logTag = "SheetsParsingUtils.ParseConfigFrames";

            var configFrameStarts = new List<ConfigFrame>();
            var configFrames = new List<ConfigFrame>();

            for (int rowIndex = 0; rowIndex < sheetData.Count; rowIndex++)
            {
                IList<object> row = sheetData[rowIndex];

                if (row == null)
                    return Result<List<ConfigFrame>, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{rowIndex + 1}' is null but shouldn't");

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
                        bool nameValid = !string.IsNullOrEmpty(name);
                        if (!nameValid)
                            return Result<List<ConfigFrame>, string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has invalid name");

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
                        bool nameValid = !string.IsNullOrEmpty(name);
                        if (!nameValid)
                            return Result<List<ConfigFrame>, string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has invalid name");

                        int matches = 0;
                        for (int configFrameIndex = configFrameStarts.Count - 1; configFrameIndex >= 0; --configFrameIndex)
                        {
                            ConfigFrame configFrame = configFrameStarts[configFrameIndex];
                            if (rowIndex >= configFrame.StartRowIndex && columnIndex >= configFrame.StartColumnIndex)
                            {
                                if (++matches > 1)
                                    return Result<List<ConfigFrame>, string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end with more than one matching starts");

                                if (!name.Equals(configFrame.Name, StringComparison.Ordinal))
                                    return Result<List<ConfigFrame>, string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end with geometrically matching start cell [{configFrame.StartRowIndex + 1}][{configFrame.StartColumnIndex + 1}] but with different name '{configFrame.Name}'");

                                configFrame.EndRowIndex = rowIndex;
                                configFrame.EndColumnIndex = columnIndex;
                                configFrameStarts.RemoveAt(configFrameIndex);
                                configFrames.Add(configFrame);
                                break;
                            }
                        }

                        if (matches == 0)
                            return Result<List<ConfigFrame>, string>.FromError($"{logTag}. Cell [{rowIndex + 1}][{columnIndex + 1}] '{cellText}' has end but there are no matching starts");
                    }
                }
            }

            if (configFrameStarts.Count > 0)
                return Result<List<ConfigFrame>, string>.FromError($"{logTag}. There are no matching ends for {configFrameStarts.Count} starts");

            return Result<List<ConfigFrame>, string>.FromValue(configFrames);
        }
    }
}