using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public sealed class VerticalSheetParserBase : SheetParserBase
    {
        public override Result<ParsedSheet, string> Parse(string sheetName, IList<IList<object>> sheetData)
        {
            IList<object> headersRow = sheetData[0];
            IList<object> typesRow = sheetData[1];

            List<ParsedPropertyInfo> properties = new List<ParsedPropertyInfo>(headersRow.Count - 1);
            for (int colIndex = 1; colIndex < headersRow.Count; colIndex++)
            {
                string propertyName = (string)headersRow[colIndex];
                string propertyType = (string)typesRow[colIndex];
                properties.Add(new ParsedPropertyInfo(propertyName, propertyType));
            }

            List<List<string>> entities = new List<List<string>>(sheetData.Count - 2);
            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
            {
                IList<object> row = sheetData[rowIndex];
                List<string> entityValues = new List<string>(properties.Count);

                for (int colIndex = 1; colIndex < row.Count; colIndex++)
                {
                    entityValues.Add((string)row[colIndex]);
                }

                entities.Add(entityValues);
            }

            var parsedSheet = new ParsedSheet(sheetName, properties, entities);
            return Result<ParsedSheet, string>.FromValue(parsedSheet);
        }

        public override string Validate(IList<IList<object>> sheetData)
        {
            if (sheetData.Count < 3)
                return
                    "Sheets parsing error. Vertical format requires at least 3 rows (headers: C4G_NAME, C4G_TYPE, and data)";

            if (sheetData[0] == null || sheetData[0].Count < 1)
                return "Sheets parsing error. First row must be not null and contain at least one column";

            if (!(sheetData[0][0] is string nameHeader) || nameHeader != SheetsParsing.NameHeader)
                return $"Sheets parsing error. First cell (A1) must be equal to '{SheetsParsing.NameHeader}'";

            if (sheetData[1] == null || sheetData[1].Count < 1)
                return "Sheets parsing error. Second row must be not null and contain at least one column";

            if (!(sheetData[1][0] is string typeHeader) || typeHeader != SheetsParsing.TypeHeader)
                return $"Sheets parsing error. Second row first cell (A2) must be equal to '{SheetsParsing.TypeHeader}'";

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