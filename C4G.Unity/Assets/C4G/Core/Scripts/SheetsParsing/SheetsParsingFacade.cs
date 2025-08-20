using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public class SheetsParsingFacade
    {
        public Result<ParsedSheet, EC4GError> ParseSheet(string sheetName, IList<IList<object>> sheetData)
        {
            EC4GError error = ValidateParameters(sheetName, sheetData);
            if (error != EC4GError.None)
                return Result<ParsedSheet, EC4GError>.FromError(error);

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
            return Result<ParsedSheet, EC4GError>.FromValue(parsedSheet);
        }

        private EC4GError ValidateParameters(string sheetName, IList<IList<object>> sheetData)
        {
            if (string.IsNullOrEmpty(sheetName))
                return EC4GError.SP_SheetNameNullOrEmpty;

            if (sheetData == null)
                return EC4GError.SP_SheetDataNull;

            if (sheetData.Count < 2)
                return EC4GError.SP_SheetDataCountLowerThanTwo;

            IList<object> headersRow = sheetData[0];

            if (headersRow == null)
                return EC4GError.SP_HeadersRowNull;

            if (headersRow.Count != 2)
                return EC4GError.SP_HeadersRowElementsCountIsNotTwo;

            if (!(headersRow[0] is string nameHeader) || nameHeader != "C4G_NAME")
                return EC4GError.SP_FirstHeaderInvalid;

            if (!(headersRow[1] is string typeHeader) || typeHeader != "C4G_TYPE")
                return EC4GError.SP_SecondHeaderInvalid;

            var firstDataRow = sheetData[1];

            if (firstDataRow == null)
                return EC4GError.SP_FirstDataRowNull;

            int firstDataRowLength = firstDataRow.Count;

            if (firstDataRowLength < 2)
                return EC4GError.SP_FirstDataRowElementsCountLowerThanTwo;

            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
            {
                var dataRow = sheetData[rowIndex];

                if (dataRow == null)
                    return EC4GError.SP_DataRowNull;

                int dataRowCount = dataRow.Count;

                if (dataRowCount != firstDataRowLength)
                {
                    return EC4GError.SP_DataRowElementsCountInvalid;
                }
            }

            return EC4GError.None;
        }
    }
}