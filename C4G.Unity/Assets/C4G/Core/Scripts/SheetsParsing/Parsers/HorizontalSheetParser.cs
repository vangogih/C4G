using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public sealed class HorizontalSheetParser : SheetParserBase
    {
        public override Result<ParsedSheet, string> Parse(string sheetName, IList<IList<object>> sheetData)
        {
            int dataRowLength = sheetData[1].Count;

            List<ParsedPropertyInfo> properties = new List<ParsedPropertyInfo>(sheetData.Count - 1);
            List<List<string>> entities = new List<List<string>>(dataRowLength - 2);

            for (var i = 0; i < dataRowLength - 2; i++)
            {
                entities.Add(new List<string>());
            }

            for (int rowIndex = 1; rowIndex < sheetData.Count; rowIndex++)
            {
                IList<object> row = sheetData[rowIndex];
                var parsedPropertyInfo = new ParsedPropertyInfo((string)row[0], (string)row[1]);
                properties.Add(parsedPropertyInfo);

                for (int insideRowIndex = 2; insideRowIndex < row.Count; insideRowIndex++)
                {
                    entities[insideRowIndex - 2].Add((string)row[insideRowIndex]);
                }
            }

            var parsedSheet = new ParsedSheet(sheetName, properties, entities);
            return Result<ParsedSheet, string>.FromValue(parsedSheet);
        }

        public override string Validate(string sheetName, IList<IList<object>> sheetData)
        {
            if (sheetData.Count < 2)
                return $"Sheets parsing error '{sheetName}'. Horizontal format requires at least 2 rows (headers and first data row)";

            IList<object> headersRow = sheetData[0];

            if (headersRow == null)
                return $"Sheets parsing error '{sheetName}'. Headers row must be not null";
            
            if (headersRow.Count != 2)
                return $"Sheets parsing error '{sheetName}'. Headers row length must be equal to two";
            
            if (!(headersRow[0] is string nameHeader) || nameHeader != SheetsParsing.NAME_HEADER)
                return $"Sheets parsing error '{sheetName}'. First header must be equal to '{SheetsParsing.NAME_HEADER}'";
            
            if (!(headersRow[1] is string typeHeader) || typeHeader != SheetsParsing.TYPE_HEADER)
                return $"Sheets parsing error '{sheetName}'. Second header must be equal to '{SheetsParsing.TYPE_HEADER}'";

            var firstDataRow = sheetData[1];

            if (firstDataRow == null)
                return $"Sheets parsing error '{sheetName}'. First data row must be not null";
            
            if (firstDataRow.Count < 2)
                return $"Sheets parsing error '{sheetName}'. First data row length must be equal or greater than two";

            int firstDataRowLength = firstDataRow.Count;

            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
            {
                var dataRow = sheetData[rowIndex];

                if (dataRow == null)
                    return $"Sheets parsing error '{sheetName}'. '{rowIndex}' data row must be not null";

                int dataRowLength = dataRow.Count;

                if (dataRowLength != firstDataRowLength)
                    return $"Sheets parsing error '{sheetName}'. '{rowIndex}' data row length '{dataRowLength}' must be equal to first data row length '{firstDataRowLength}'";
            }

            return string.Empty;
        }
    }
}