using System.Collections.Generic;
using C4G.Core.Settings;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public sealed class SheetsParsing
    {
        private const string TYPE_HEADER = "C4G_TYPE";
        private const string NAME_HEADER = "C4G_NAME";

        private readonly Dictionary<ParsingType, ISheetParser> _parsers;

        public SheetsParsing()
        {
            _parsers = new Dictionary<ParsingType, ISheetParser>
            {
                { ParsingType.Horizontal, new HorizontalSheetParser() },
                { ParsingType.Vertical, new VerticalSheetParser() }
            };
        }

        public Result<ParsedSheet, string> ParseSheet(SheetInfo sheetInfo, IList<IList<object>> sheetData)
        {
            bool isValid = ValidateParameters(sheetInfo, sheetData, out string error);
            if (!isValid)
                return Result<ParsedSheet, string>.FromError(error);

            if (!_parsers.TryGetValue(sheetInfo.parsingType, out ISheetParser parser))
                return Result<ParsedSheet, string>.FromError($"Sheets parsing error. Unsupported parsing type: {sheetInfo.parsingType}");

            return parser.Parse(sheetInfo.sheetName, sheetData);
        }

        private bool ValidateParameters(SheetInfo sheetInfo, IList<IList<object>> sheetData, out string error)
        {
            error = string.Empty;

            if (sheetInfo == null)
                error = "Sheets parsing error. Sheet info must be not null";
            else if (string.IsNullOrEmpty(sheetInfo.sheetName))
                error = "Sheets parsing error. Sheet name must be not null or empty";
            else if (sheetData == null)
                error = "Sheets parsing error. Sheet data must be not null";
            else if (sheetData.Count < 2)
                error = "Sheets parsing error. Sheet data length must be equal or greater than two";
            else
            {
                if (sheetInfo.parsingType == ParsingType.Horizontal)
                    error = ValidateHorizontalFormat(sheetData);
                else if (sheetInfo.parsingType == ParsingType.Vertical)
                    error = ValidateVerticalFormat(sheetData);
                else
                    error = $"Sheets parsing error. Unknown parsing type: {sheetInfo.parsingType}";
            }

            return string.IsNullOrEmpty(error);
        }

        private string ValidateHorizontalFormat(IList<IList<object>> sheetData)
        {
            IList<object> headersRow = sheetData[0];

            if (headersRow == null)
                return "Sheets parsing error. Headers row must be not null";
            
            if (headersRow.Count != 2)
                return "Sheets parsing error. Headers row length must be equal to two";
            
            if (!(headersRow[0] is string nameHeader) || nameHeader != NAME_HEADER)
                return $"Sheets parsing error. First header must be equal to '{NAME_HEADER}'";
            
            if (!(headersRow[1] is string typeHeader) || typeHeader != TYPE_HEADER)
                return $"Sheets parsing error. Second header must be equal to '{TYPE_HEADER}'";

            var firstDataRow = sheetData[1];

            if (firstDataRow == null)
                return "Sheets parsing error. First data row must be not null";
            
            if (firstDataRow.Count < 2)
                return "Sheets parsing error. First data row length must be equal or greater than two";

            int firstDataRowLength = firstDataRow.Count;

            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
            {
                var dataRow = sheetData[rowIndex];

                if (dataRow == null)
                    return $"Sheets parsing error. '{rowIndex}' data row must be not null";

                int dataRowLength = dataRow.Count;

                if (dataRowLength != firstDataRowLength)
                    return $"Sheets parsing error. '{rowIndex}' data row length '{dataRowLength}' must be equal to first data row length '{firstDataRowLength}'";
            }

            return string.Empty;
        }

        private string ValidateVerticalFormat(IList<IList<object>> sheetData)
        {
            if (sheetData.Count < 3)
                return
                    "Sheets parsing error. Vertical format requires at least 3 rows (headers: C4G_NAME, C4G_TYPE, and data)";

            if (sheetData[0] == null || sheetData[0].Count < 1)
                return "Sheets parsing error. First row must be not null and contain at least one column";

            if (!(sheetData[0][0] is string nameHeader) || nameHeader != NAME_HEADER)
                return $"Sheets parsing error. First cell (A1) must be equal to '{NAME_HEADER}'";

            if (sheetData[1] == null || sheetData[1].Count < 1)
                return "Sheets parsing error. Second row must be not null and contain at least one column";

            if (!(sheetData[1][0] is string typeHeader) || typeHeader != TYPE_HEADER)
                return $"Sheets parsing error. Second row first cell (A2) must be equal to '{TYPE_HEADER}'";

            int firstRowLength = sheetData[0].Count;

            if (firstRowLength < 2)
                return
                    "Sheets parsing error. Each row must have at least 2 columns (C4G_NAME/C4G_TYPE + at least one property)";

            for (int rowIndex = 1; rowIndex < sheetData.Count; rowIndex++)
            {
                var row = sheetData[rowIndex];

                if (row == null)
                    return $"Sheets parsing error. Row '{rowIndex}' must be not null";

                if (row.Count != firstRowLength)
                    return
                        $"Sheets parsing error. Row '{rowIndex}' length '{row.Count}' must be equal to first row length '{firstRowLength}'";
            }

            return string.Empty;
        }
    }
}