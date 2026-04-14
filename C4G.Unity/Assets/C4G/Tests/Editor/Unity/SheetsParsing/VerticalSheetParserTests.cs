using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
    [TestFixture]
    public class VerticalSheetParserTests
    {
        private SheetsParsingFacade _sheetsParsingFacade;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsingFacade = new SheetsParsingFacade();
        }

        [Test]
        public void ParseSheet_Positive_UsualCase()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Name", "Rank", "Range", "Slots" },
                new List<object> { "int", "string", "float", "double", "List<int>" },
                new List<object> { "1", "Alice", "5.3", "8.5", "1,2,3" },
                new List<object> { "2", "Bob", "6.6", "35.1", "5,6,8" },
                new List<object> { "3", "Charlie", "3.5", "33", "3,5" }
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual(sheetName, parsedConfig.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfig.Entities);
        }

        [Test]
        public void ParseSheet_Positive_SingleEntity()
        {
            // Arrange
            string sheetName = "SingleEntity";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Name" },
                new List<object> { "int", "string" },
                new List<object> { "100", "Test" }
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual(sheetName, parsedConfig.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfig.Entities);
        }

        [Test]
        public void ParseSheet_Positive_SingleProperty()
        {
            // Arrange
            string sheetName = "SingleProperty";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id" },
                new List<object> { "int" },
                new List<object> { "1" },
                new List<object> { "2" },
                new List<object> { "3" }
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual(sheetName, parsedConfig.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfig.Entities);
        }

        [Test]
        public void ParseSheet_Positive_ManyProperties()
        {
            // Arrange
            string sheetName = "ManyProperties";
            var sheetData = new List<IList<object>>
            {
                new List<object>
                {
                    "Prop1", "Prop2", "Prop3", "Prop4", "Prop5", "Prop6", "Prop7", "Prop8", "Prop9", "Prop10"
                },
                new List<object>
                {
                    "int", "string", "float", "double", "bool", "int", "string", "float", "double", "bool"
                },
                new List<object> { "1", "a", "1.1", "2.2", "true", "10", "aa", "11.1", "22.2", "false" },
                new List<object> { "2", "b", "3.3", "4.4", "false", "20", "bb", "33.3", "44.4", "true" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual(10, parsedConfig.Properties.Count);
            Assert.AreEqual(2, parsedConfig.Entities.Count);
            Assert.AreEqual(10, parsedConfig.Entities[0].Count);
            Assert.AreEqual(10, parsedConfig.Entities[1].Count);
        }

        [Test]
        public void ParseSheet_Positive_ManyEntities()
        {
            // Arrange
            string sheetName = "ManyEntities";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Name" },
                new List<object> { "int", "string" }
            };

            // Add 100 entities
            for (int i = 0; i < 100; i++)
            {
                sheetData.Add(new List<object> { i.ToString(), $"Name{i}" });
            }

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual(2, parsedConfig.Properties.Count);
            Assert.AreEqual(100, parsedConfig.Entities.Count);
        }

        [Test]
        public void ParseSheet_Negative_InsufficientRows()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var twoRowsOnly = new List<IList<object>>
            {
                new List<object> { "Id" },
                new List<object> { "int" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(validSheetName, twoRowsOnly, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("< 3"));
        }

        [Test]
        public void ParseSheet_Negative_InsufficientColumns()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var emptyColumns = new List<IList<object>>
            {
                new List<object> { },
                new List<object> { },
                new List<object> { }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(validSheetName, emptyColumns, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("< 1"));
        }

        [Test]
        public void ParseSheet_Negative_InconsistentRowLengths()
        {
            // Arrange
            string validSheetName = "TestSheet";

            var inconsistentData = new List<IList<object>>
            {
                new List<object> { "Id", "Name", "Rank" },
                new List<object> { "int", "string", "float" },
                new List<object> { "1", "Alice" } // Missing column
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(validSheetName, inconsistentData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("< expected"));
        }

        [Test]
        public void ParseSheet_Negative_EmptyPropertyName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "", "Name" },
                new List<object> { "int", "string" },
                new List<object> { "1", "Alice" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("must contain property name"));
        }

        [Test]
        public void ParseSheet_Negative_EmptyPropertyType()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Name" },
                new List<object> { "", "string" },
                new List<object> { "1", "Alice" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("must contain property type"));
        }

        [Test]
        public void ParseSheet_Destructive_NullPropertyName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { null, "Name" },
                new List<object> { "int", "string" },
                new List<object> { "1", "Alice" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("must contain property name"));
        }

        [Test]
        public void ParseSheet_Destructive_NullPropertyType()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Name" },
                new List<object> { null, "string" },
                new List<object> { "1", "Alice" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsingFacade.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("must contain property type"));
        }
    }
}