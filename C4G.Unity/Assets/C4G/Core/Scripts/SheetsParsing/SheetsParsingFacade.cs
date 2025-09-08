using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public class SheetsParsingFacade
    {
        private string TYPE_HEADER;
        private const string NAME_HEADER = "C4G_NAME";

        public Result<ParsedSheet, C4GError> ParseSheet(string sheetName, IList<IList<object>> sheetData)
        {
            bool isParametersValid = CheckIfParametersValid(sheetName, sheetData, out C4GError error);
            if (!isParametersValid)
                return Result<ParsedSheet, C4GError>.FromError(error);

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
            return Result<ParsedSheet, C4GError>.FromValue(parsedSheet);
        }

        private bool CheckIfParametersValid(string sheetName, IList<IList<object>> sheetData, out C4GError error)
        {
            error = default;

            if (string.IsNullOrEmpty(sheetName))
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "Sheet name must be not null or empty.");
                return false;
            }

            if (sheetData == null)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "Sheet data must be notnull.");
                return false;
            }

            if (sheetData.Count < 2)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "Sheet data length must be equal or greater than two.");
                return false;
            }

            IList<object> headersRow = sheetData[0];

            if (headersRow == null)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "Headers row must be not null.");
                return false;
            }

            if (headersRow.Count != 2)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "Headers row length must be equal to two.");
                return false;
            }

            if (!(headersRow[0] is string nameHeader) || nameHeader != NAME_HEADER)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, $"First header must be equal to '{NAME_HEADER}'");
                return false;
            }

            TYPE_HEADER = "C4G_TYPE";
            if (!(headersRow[1] is string typeHeader) || typeHeader != TYPE_HEADER)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, $"Second header must be equal to '{TYPE_HEADER}'");
                return false;
            }

            var firstDataRow = sheetData[1];

            if (firstDataRow == null)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "First data row must be not null");
                return false;
            }

            int firstDataRowLength = firstDataRow.Count;

            if (firstDataRowLength < 2)
            {
                error = new C4GError(EC4GErrorType.SheetsParsing, "First data row length must be equal or greater than two.");
                return false;
            }

            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
            {
                var dataRow = sheetData[rowIndex];

                if (dataRow == null)
                {
                    error = new C4GError(EC4GErrorType.SheetsParsing, $"'{rowIndex}' data row length must be not null.");
                    return false;
                }

                int dataRowLength = dataRow.Count;

                if (dataRowLength != firstDataRowLength)
                {
                    error = new C4GError(EC4GErrorType.SheetsParsing, $"'{rowIndex}' data row length '{dataRowLength}' must be equal to first data row length '{firstDataRowLength}'.");
                    return false;
                }
            }

            return true;
        }
    }
}