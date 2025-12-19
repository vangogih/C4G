using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
    [TestFixture]
    public class VerticalSheetParserTests
    {
        private Core.SheetsParsing.SheetsParsing _sheetsParsing;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsing = new Core.SheetsParsing.SheetsParsing();
        }

        [Test]
        public void ParseSheet_VerticalFormat_UsualCase()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id", "Name", "Rank", "Range", "Slots" },
                new List<object> { "C4G_TYPE", "int", "string", "float", "double", "List<int>" },
                new List<object> { "Entity1", "1", "Alice", "5.3", "8.5", "1,2,3" },
                new List<object> { "Entity2", "2", "Bob", "6.6", "35.1", "5,6,8" },
                new List<object> { "Entity3", "3", "Charlie", "3.5", "33", "3,5" }
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
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_VerticalFormat_SingleEntity()
        {
            // Arrange
            string sheetName = "SingleEntity";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id", "Name" },
                new List<object> { "C4G_TYPE", "int", "string" },
                new List<object> { "Entity1", "100", "Test" }
            };
            var expectedProperties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Name", "string")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "100", "Test" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_VerticalFormat_SingleProperty()
        {
            // Arrange
            string sheetName = "SingleProperty";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id" },
                new List<object> { "C4G_TYPE", "int" },
                new List<object> { "Entity1", "1" },
                new List<object> { "Entity2", "2" },
                new List<object> { "Entity3", "3" }
            };
            var expectedProperties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1" },
                new List<string> { "2" },
                new List<string> { "3" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_VerticalFormat_InsufficientRows()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var twoRowsOnly = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id" },
                new List<object> { "C4G_TYPE", "int" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(validSheetName, twoRowsOnly, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("at least 3 rows"));
        }

        [Test]
        public void ParseSheet_VerticalFormat_InsufficientColumns()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var oneColumnOnly = new List<IList<object>>
            {
                new List<object> { "C4G_NAME" },
                new List<object> { "C4G_TYPE" },
                new List<object> { "Entity1" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(validSheetName, oneColumnOnly, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("at least 2 columns"));
        }

        [Test]
        public void ParseSheet_VerticalFormat_InconsistentRowLengths()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var inconsistentData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id", "Name", "Rank" },
                new List<object> { "C4G_TYPE", "int", "string", "float" },
                new List<object> { "Entity1", "1", "Alice" } // Missing column
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(validSheetName, inconsistentData, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("must be equal to first row length"));
        }

        [Test]
        public void ParseSheet_VerticalFormat_ManyProperties()
        {
            // Arrange
            string sheetName = "ManyProperties";
            var sheetData = new List<IList<object>>
            {
                new List<object>
                {
                    "C4G_NAME", "Prop1", "Prop2", "Prop3", "Prop4", "Prop5", "Prop6", "Prop7", "Prop8", "Prop9",
                    "Prop10"
                },
                new List<object>
                {
                    "C4G_TYPE", "int", "string", "float", "double", "bool", "int", "string", "float", "double", "bool"
                },
                new List<object> { "Entity1", "1", "a", "1.1", "2.2", "true", "10", "aa", "11.1", "22.2", "false" },
                new List<object> { "Entity2", "2", "b", "3.3", "4.4", "false", "20", "bb", "33.3", "44.4", "true" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual(10, parsedSheet.Properties.Count);
            Assert.AreEqual(2, parsedSheet.Entities.Count);
            Assert.AreEqual(10, parsedSheet.Entities[0].Count);
            Assert.AreEqual(10, parsedSheet.Entities[1].Count);
        }

        [Test]
        public void ParseSheet_VerticalFormat_ManyEntities()
        {
            // Arrange
            string sheetName = "ManyEntities";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id", "Name" },
                new List<object> { "C4G_TYPE", "int", "string" }
            };

            // Add 100 entities
            for (int i = 0; i < 100; i++)
            {
                sheetData.Add(new List<object> { $"Entity{i}", i.ToString(), $"Name{i}" });
            }

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new VerticalSheetParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual(2, parsedSheet.Properties.Count);
            Assert.AreEqual(100, parsedSheet.Entities.Count);
        }
    }
}