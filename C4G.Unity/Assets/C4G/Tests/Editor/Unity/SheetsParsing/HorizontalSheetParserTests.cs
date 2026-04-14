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
            var expectedProperties = new ParsedPropertyInfo[]
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new HorizontalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual(sheetName, parsedConfig.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfig.Entities);
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
            var parsedConfigsBuffer = new List<ParsedConfig>();

            // Act
            var invalidDataLengthResult = _sheetsParsing.ParseSheetToList(validSheetName, invalidDataLengthData, parser, parsedConfigsBuffer);
            var nullSheetNameResult = _sheetsParsing.ParseSheetToList(null, validSheetData, parser, parsedConfigsBuffer);
            var nullSheetDataResult = _sheetsParsing.ParseSheetToList(validSheetName, null, parser, parsedConfigsBuffer);
            var emptySheetResult = _sheetsParsing.ParseSheetToList(validSheetName, emptySheetData, parser, parsedConfigsBuffer);

            // Assert
            Assert.IsFalse(invalidDataLengthResult.IsOk);
            Assert.IsFalse(nullSheetNameResult.IsOk);
            Assert.IsFalse(nullSheetDataResult.IsOk);
            Assert.IsFalse(emptySheetResult.IsOk);
        }
    }
}