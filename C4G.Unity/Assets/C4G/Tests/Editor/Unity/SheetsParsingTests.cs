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

        #region HorizontalParsingTests

        [Test]
        public void ParseSheet_UsualCase()
        {
            // Arrange
            string sheetName = "TestSheet";
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
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData, new HorizontalSheetParser());

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
            string validSheetName = "TestSheet";
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

            var parser = new HorizontalSheetParser();

            var invalidHeaderResult = _sheetsParsing.ParseSheet(validSheetName, invalidHeaderData, parser);
            var invalidDataLengthResult = _sheetsParsing.ParseSheet(validSheetName, invalidDataLengthData, parser);
            var nullSheetNameResult = _sheetsParsing.ParseSheet(null, validSheetData, parser);
            var nullSheetDataResult = _sheetsParsing.ParseSheet(validSheetName, null, parser);
            var emptySheetResult = _sheetsParsing.ParseSheet(validSheetName, emptySheetData, parser);

            // Assert
            Assert.IsFalse(invalidHeaderResult.IsOk);
            Assert.IsFalse(invalidDataLengthResult.IsOk);
            Assert.IsFalse(nullSheetNameResult.IsOk);
            Assert.IsFalse(nullSheetDataResult.IsOk);
            Assert.IsFalse(emptySheetResult.IsOk);
        }

        #endregion

        #region VerticalParsingTests

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
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData, new VerticalSheetParser());

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
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
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData, new VerticalSheetParser());

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
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
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData, new VerticalSheetParser());

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
            Assert.AreEqual(sheetName, parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_VerticalFormat_InvalidHeaders()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var invalidFirstHeader = new List<IList<object>>
            {
                new List<object> { "INVALID_NAME", "Id", "Name" },
                new List<object> { "C4G_TYPE", "int", "string" },
                new List<object> { "Entity1", "1", "Alice" }
            };

            var invalidSecondHeader = new List<IList<object>>
            {
                new List<object> { "C4G_NAME", "Id", "Name" },
                new List<object> { "INVALID_TYPE", "int", "string" },
                new List<object> { "Entity1", "1", "Alice" }
            };

            // Act
            var invalidFirstResult =
                _sheetsParsing.ParseSheet(validSheetName, invalidFirstHeader, new VerticalSheetParser());
            var invalidSecondResult =
                _sheetsParsing.ParseSheet(validSheetName, invalidSecondHeader, new VerticalSheetParser());

            // Assert
            Assert.IsFalse(invalidFirstResult.IsOk);
            Assert.IsTrue(invalidFirstResult.Error.Contains("C4G_NAME"));
            Assert.IsFalse(invalidSecondResult.IsOk);
            Assert.IsTrue(invalidSecondResult.Error.Contains("C4G_TYPE"));
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
            var result = _sheetsParsing.ParseSheet(validSheetName, twoRowsOnly, new VerticalSheetParser());

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
            var result = _sheetsParsing.ParseSheet(validSheetName, oneColumnOnly, new VerticalSheetParser());

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
            var result = _sheetsParsing.ParseSheet(validSheetName, inconsistentData, new VerticalSheetParser());

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
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData, new VerticalSheetParser());

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
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
            var result = _sheetsParsing.ParseSheet(sheetName, sheetData, new VerticalSheetParser());

            // Assert
            Assert.IsTrue(result.IsOk);
            var parsedSheet = result.Value;
            Assert.AreEqual(2, parsedSheet.Properties.Count);
            Assert.AreEqual(100, parsedSheet.Entities.Count);
        }

        #endregion
    }
}