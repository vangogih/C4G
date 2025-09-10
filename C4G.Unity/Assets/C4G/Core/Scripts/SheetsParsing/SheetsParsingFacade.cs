using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public class SheetsParsingFacade
    {
        private const string TYPE_HEADER = "C4G_TYPE";
        private const string NAME_HEADER = "C4G_NAME";

        public Result<ParsedSheet, string> ParseSheet(string sheetName, IList<IList<object>> sheetData)
        {
            bool isValid = ValidateParameters(sheetName, sheetData, out string error);
            if (!isValid)
                return Result<ParsedSheet, string>.FromError(error);

            int dataRowLength = sheetData[1].Count;

            List<ParsedPropertyInfo> properties = new List<ParsedPropertyInfo>(sheetData.Count - 1);
            List<List<string>> entities = new List<List<string>>(sheetData.Count - 1);

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

        private bool ValidateParameters(string sheetName, IList<IList<object>> sheetData, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(sheetName))
                error = "Sheets parsing error. Sheet name must be not null or empty";
            else if (sheetData == null)
                error = "Sheets parsing error. Sheet data must be not null";
            else if (sheetData.Count < 2)
                error = "Sheets parsing error. Sheet data length must be equal or greater than two";
            else
            {
                IList<object> headersRow = sheetData[0];

                if (headersRow == null)
                    error = "Sheets parsing error. Headers row must be not null";
                else if (headersRow.Count != 2)
                    error = "Sheets parsing error. Headers row length must be equal to two";
                else if (!(headersRow[0] is string nameHeader) || nameHeader != NAME_HEADER)
                    error = $"Sheets parsing error. First header must be equal to '{NAME_HEADER}'";
                else
                {
                    if (!(headersRow[1] is string typeHeader) || typeHeader != TYPE_HEADER)
                        error = $"Sheets parsing error. Second header must be equal to '{TYPE_HEADER}'";
                    else
                    {
                        var firstDataRow = sheetData[1];

                        if (firstDataRow == null)
                            error = "Sheets parsing error. First data row must be not null";
                        else if (firstDataRow.Count < 2)
                            error = "Sheets parsing error. First data row length must be equal or greater than two";
                        else
                        {
                            int firstDataRowLength = firstDataRow.Count;

                            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
                            {
                                var dataRow = sheetData[rowIndex];

                                if (dataRow == null)
                                {
                                    error = $"Sheets parsing error. '{rowIndex}' data row must be not null";
                                    break;
                                }

                                int dataRowLength = dataRow.Count;

                                if (dataRowLength != firstDataRowLength)
                                {
                                    error = $"Sheets parsing error. '{rowIndex}' data row length '{dataRowLength}' must be equal to first data row length '{firstDataRowLength}'";
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return string.IsNullOrEmpty(error);
        }
    }
}