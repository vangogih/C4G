using System;
using System.Collections.Generic;

namespace C4G.Editor
{
    public sealed class ParsedPropertyInfo
    {
        public readonly string Name;
        public readonly string Type;

        public ParsedPropertyInfo(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }

    public sealed class ParsedSheet
    {
        public readonly string Name;
        public readonly IReadOnlyCollection<ParsedPropertyInfo> Properties;
        public readonly IReadOnlyCollection<IReadOnlyCollection<string>> Entities;

        public ParsedSheet(string name, IReadOnlyCollection<ParsedPropertyInfo> properties, IReadOnlyCollection<IReadOnlyCollection<string>> entities)
        {
            Name = name;
            Properties = properties;
            Entities = entities;
        }
    }

    public static class SheetParser
    {
        public static ParsedSheet ParseSheet(string sheetName, IList<IList<object>> sheetData)
        {
            if (sheetData.Count < 2)
                throw new Exception($"At least two rows required in a sheet {sheetName}");
            
            IList<object> headersRow = sheetData[0];
            if (headersRow.Count != 2)
                throw new Exception($"Only two elements expected in the first row but got {headersRow.Count}");

            if (!(headersRow[0] is string nameHeader) || nameHeader != "C4G_NAME")
                throw new Exception($"Expected C4G_NAME first header, but got {headersRow[0]}");

            if (!(headersRow[1] is string typeHeader) || typeHeader != "C4G_TYPE")
                throw new Exception($"Expected C4G_TYPE second header, but got {headersRow[1]}");

            int expectedLenght = sheetData[1].Count;

            if (expectedLenght < 2)
                throw new Exception($"Expected row two length two or greater, but got {expectedLenght}");
            
            for (int rowIndex = 2; rowIndex < sheetData.Count; rowIndex++)
            {
                if (sheetData[rowIndex].Count != expectedLenght)
                {
                    throw new Exception($"Non uniform row with length {sheetData[rowIndex].Count} found at index {rowIndex}." +
                                        $"Expected length as second row - {expectedLenght}");
                }
            }
            
            List<ParsedPropertyInfo> properties = new List<ParsedPropertyInfo>(sheetData.Count - 1);
            List<List<string>> entities = new List<List<string>>(sheetData.Count - 1);

            for (var i = 0; i < expectedLenght - 2; i++)
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

            return new ParsedSheet(sheetName, properties, entities);
        }
    }
}