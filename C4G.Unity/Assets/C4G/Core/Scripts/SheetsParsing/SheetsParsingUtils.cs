using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    internal static class SheetsParsingUtils
    {
        internal static Result<ParsedSheet, string> ParseHorizontal(string sheetName, IList<IList<object>> sheetData, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        {
            string validationError = ValidateIndices(sheetName, startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
            if (!string.IsNullOrEmpty(validationError))
                return Result<ParsedSheet, string>.FromError(validationError);

            if (sheetData.Count <= endRowIndex)
                return Result<ParsedSheet, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Rows amount '{sheetData.Count}' < expected '{endRowIndex + 1}'");

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
                    return Result<ParsedSheet, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Row '{rowIndex + 1}' length '{row.Count}' < expected '{endColumnIndex + 1}'");

                string propertyName = (string)row[startColumnIndex];
                if (string.IsNullOrEmpty(propertyName))
                    return Result<ParsedSheet, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Cell [{rowIndex + 1}][{startColumnIndex + 1}] must contain property name, but has null or empty value instead");

                string propertyType = (string)row[startColumnIndex + 1];
                if (string.IsNullOrEmpty(propertyType))
                    return Result<ParsedSheet, string>.FromError($"C4G Error. Sheet name '{sheetName}'. Cell [{rowIndex + 1}][{startColumnIndex + 2}] must contain property type, but has null or empty value instead");

                var parsedPropertyInfo = new ParsedPropertyInfo(propertyName, propertyType);
                properties[rowIndex - startRowIndex] = parsedPropertyInfo;

                for (int columnIndex = startColumnIndex + 2; columnIndex <= endColumnIndex; columnIndex++)
                {
                    entities[columnIndex - startColumnIndex - 2].Add((string)row[columnIndex]);
                }
            }

            var parsedSheet = new ParsedSheet(sheetName, properties, entities);
            return Result<ParsedSheet, string>.FromValue(parsedSheet);
        }

        private static string ValidateIndices(string sheetName, int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
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
    }
}