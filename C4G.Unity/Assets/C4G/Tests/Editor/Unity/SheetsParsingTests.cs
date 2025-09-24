using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class SheetsParsingTests
    {
        private SheetsParsing _sheetsParsing;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsing = new SheetsParsing();
        }

        [Test]
        public void ParseSheet_UsualCase()
        {
            // Arrange
            var sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1", "2", "3" },
                new List<object> { "Name", "string", "Alice", "Bob", "Charlie" },
                new List<object> { "Rank", "float", "5.3", "6.6", "3.5" },
                new List<object> { "Range", "double", "8.5", "35.1", "33" },
                new List<object> { "Slots", "List<int>", "1,2,3", "5,6,8", "3,5" }
            };
            var expectedProperties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Name", "string"),
                new ParsedPropertyInfo("Rank", "float"),
                new ParsedPropertyInfo("Range", "double"),
                new ParsedPropertyInfo("Slots", "List<int>")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1", "Alice", "5.3", "8.5", "1,2,3" },
                new List<string> { "2", "Bob", "6.6", "35.1", "5,6,8" },
                new List<string> { "3", "Charlie", "3.5", "33", "3,5" }
            };

            // Act
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData);

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_WrongInputLeadsToError()
        {
            // Arrange
            var validSheetName = "TestSheet";
            var invalidHeaderData = new List<IList<object>>
            {
                new List<object> { "INVALID_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" }
            };
            var invalidDataLengthData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" },
                new List<object> { "Name", "string" } // Data row length mismatch
            };
            var validSheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "C4G_TYPE" },
                new List<object> { "Id", "int", "1" }
            };
            var emptySheetData = new List<IList<object>>();

            // Act
            var invalidHeaderResult = _sheetsParsing.ParseSheet(validSheetName, invalidHeaderData);
            var invalidDataLengthResult = _sheetsParsing.ParseSheet(validSheetName, invalidDataLengthData);
            var nullSheetNameResult = _sheetsParsing.ParseSheet(null, validSheetData);
            var nullSheetDataResult = _sheetsParsing.ParseSheet(validSheetName, null);
            var emptySheetResult = _sheetsParsing.ParseSheet(validSheetName, emptySheetData);

            // Assert
            Assert.IsFalse(invalidHeaderResult.IsOk);
            Assert.IsFalse(invalidDataLengthResult.IsOk);
            Assert.IsFalse(nullSheetNameResult.IsOk);
            Assert.IsFalse(nullSheetDataResult.IsOk);
            Assert.IsFalse(emptySheetResult.IsOk);
        }
    }
}