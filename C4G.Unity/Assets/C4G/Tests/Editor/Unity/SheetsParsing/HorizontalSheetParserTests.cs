using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
    [TestFixture]
    public class HorizontalSheetParserTests
    {
        private Core.SheetsParsing.SheetsParsing _sheetsParsing;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsing = new Core.SheetsParsing.SheetsParsing();
        }

        [Test]
        public void ParseSheet_UsualCase()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
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
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalSheetParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_WrongInputLeadsToError()
        {
            // Arrange
            string validSheetName = "TestSheet";
            var invalidDataLengthData = new List<IList<object>>
            {
                new List<object> { "Id", "int", "1" },
                new List<object> { "Name", "string" } // Data row length mismatch
            };
            var validSheetData = new List<IList<object>>
            {
                new List<object> { "Id", "int", "1" }
            };
            var emptySheetData = new List<IList<object>>();
            var parser = new HorizontalSheetParser();
            var sheetsBuffer = new List<ParsedSheet>();

            // Act
            var invalidDataLengthResult = _sheetsParsing.ParseSheetNonAlloc(validSheetName, invalidDataLengthData, parser, sheetsBuffer);
            var nullSheetNameResult = _sheetsParsing.ParseSheetNonAlloc(null, validSheetData, parser, sheetsBuffer);
            var nullSheetDataResult = _sheetsParsing.ParseSheetNonAlloc(validSheetName, null, parser, sheetsBuffer);
            var emptySheetResult = _sheetsParsing.ParseSheetNonAlloc(validSheetName, emptySheetData, parser, sheetsBuffer);

            // Assert
            Assert.IsFalse(invalidDataLengthResult.IsOk);
            Assert.IsFalse(nullSheetNameResult.IsOk);
            Assert.IsFalse(nullSheetDataResult.IsOk);
            Assert.IsFalse(emptySheetResult.IsOk);
        }
    }
}