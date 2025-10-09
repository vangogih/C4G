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
    }
}