using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
    [TestFixture]
    public class HorizontalManySheetsOnOneParserTests
    {
        private Core.SheetsParsing.SheetsParsing _sheetsParsing;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsing = new Core.SheetsParsing.SheetsParsing();
        }

        #region Positive Cases

        [Test]
        public void ParseSheet_Positive_SingleValidConfig()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.users" },
                new List<object> { ""              , "id"   , "int"    , "1"    , "123"  , "456" },
                new List<object> { ""              , "name" , "string" , "Alex" , "Dima" , "Serghei" },
                new List<object> { ""              , "age"  , "float"  , "27.6" , "12.1" , "13.5" },
                new List<object> { ""              , ""     , ""       , ""     , ""     , ""          , "end.users" }
            };

            var expectedProperties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("id", "int"),
                new ParsedPropertyInfo("name", "string"),
                new ParsedPropertyInfo("age", "float")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1", "Alex", "27.6" },
                new List<string> { "123", "Dima", "12.1" },
                new List<string> { "456", "Serghei", "13.5" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(1, parsedSheets.Count);
            var parsedSheet = parsedSheets[0];
            Assert.AreEqual("users", parsedSheet.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheet.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheet.Entities);
        }

        [Test]
        public void ParseSheet_Positive_OneValidConfigUnderAnotherValidConfig()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                // Config A
                new List<object> { "start.A" },
                new List<object> { ""          , "field1" , "type1" , "val1" , "val2" },
                new List<object> { ""          , ""       , ""      , ""     , ""       , "end.A" },

                // Empty row
                new List<object> { "" },

                // Config B
                new List<object> { "start.B" },
                new List<object> { ""          , "id"   , "int"    , "10" , "20" , "30" },
                new List<object> { ""          , "name" , "string" , "X"  , "Y"  , "Z" },
                new List<object> { ""          , ""     , ""       , ""   , ""   , ""     , "end.B" }
            };
            var expectedPropertiesA = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("field1", "type1")
            };
            var expectedEntitiesA = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "val1" },
                new List<string> { "val2" }
            };
            var expectedPropertiesB = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("id", "int"),
                new ParsedPropertyInfo("name", "string")
            };
            var expectedEntitiesB = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "10", "X" },
                new List<string> { "20", "Y" },
                new List<string> { "30", "Z" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(2, parsedSheets.Count);
            Assert.AreEqual("A", parsedSheets[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesA, parsedSheets[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesA, parsedSheets[0].Entities);
            Assert.AreEqual("B", parsedSheets[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesB, parsedSheets[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesB, parsedSheets[1].Entities);
        }

        [Test]
        public void ParseSheet_Positive_TwoConfigsWithSharedCorner()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                // Config A
                new List<object> { "start.A" },
                new List<object> { ""          , "x"   , "int" , "1"  , "2" },
                new List<object> { ""          , ""    , ""    , ""   , ""    , "end.A" },

                // Config B
                new List<object> { ""          , ""    , ""    , ""   , ""    , ""        , "start.B" },
                new List<object> { ""          , ""    , ""    , ""   , ""    , ""        , ""         , "y"    , "string" , "alpha" , "beta" },
                new List<object> { ""          , ""    , ""    , ""   , ""    , ""        , ""         , ""     , ""       , ""      , ""       , "end.B" }
            };
            var expectedPropertiesA = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("x", "int")
            };
            var expectedEntitiesA = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1" },
                new List<string> { "2" }
            };
            var expectedPropertiesB = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("y", "string")
            };
            var expectedEntitiesB = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "alpha" },
                new List<string> { "beta" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(2, parsedSheets.Count);
            Assert.AreEqual("A", parsedSheets[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesA, parsedSheets[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesA, parsedSheets[0].Entities);
            Assert.AreEqual("B", parsedSheets[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesB, parsedSheets[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesB, parsedSheets[1].Entities);
        }

        [Test]
        public void ParseSheet_Positive_ConfigsWithDiagonalArrangement()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                // Config A - top left
                new List<object> { "start.A" },
                new List<object> { ""          , "id" , "int" , "100" },
                new List<object> { ""          , ""   , ""    , ""      , "end.A" },

                // Empty row
                new List<object> { "" },

                // Config B
                new List<object> { ""          , ""    , ""    , "start.B" },
                new List<object> { ""          , ""    , ""    , ""         , "name"   , "string" , "John" , "Jane" },
                new List<object> { ""          , ""    , ""    , ""         , "active" , "bool"   , "true" , "false" },
                new List<object> { ""          , ""    , ""    , ""         , ""       , ""       , ""     , ""       , "end.B" }
            };
            var expectedPropertiesA = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("id", "int")
            };
            var expectedEntitiesA = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "100" }
            };
            var expectedPropertiesB = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("name", "string"),
                new ParsedPropertyInfo("active", "bool")
            };
            var expectedEntitiesB = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "John", "true" },
                new List<string> { "Jane", "false" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(2, parsedSheets.Count);
            Assert.AreEqual("A", parsedSheets[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesA, parsedSheets[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesA, parsedSheets[0].Entities);
            Assert.AreEqual("B", parsedSheets[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesB, parsedSheets[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesB, parsedSheets[1].Entities);
        }

        [Test]
        public void ParseSheet_Positive_ComplexScenarioWithMultipleConfigs()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                // Config users - large config top left
                new List<object> { "start.users" },
                new List<object> { ""              , "id"     , "int"    , "1"      , "2"     , "3" },
                new List<object> { ""              , "name"   , "string" , "Alice"  , "Bob"   , "Charlie" },
                new List<object> { ""              , "age"    , "int"    , "25"     , "30"    , "35" },
                new List<object> { ""              , "salary" , "float"  , "50.5"   , "75.3"  , "90.1" },
                new List<object> { ""              , ""       , ""       , ""       , ""      , ""          , "end.users" },

                // Empty row
                new List<object> { "" },

                // Config settings - small config middle
                new List<object> { ""              , ""       , ""       , "start.settings" },
                new List<object> { ""              , ""       , ""       , ""                , "key"    , "string" , "theme"  , "lang" },
                new List<object> { ""              , ""       , ""       , ""                , "value"  , "string" , "dark"   , "en" },
                new List<object> { ""              , ""       , ""       , ""                , ""       , ""       , ""       , ""       , "end.settings" },

                // Empty rows
                new List<object> { "" },
                new List<object> { "" },

                // Config meta - tiny config bottom left
                new List<object> { "start.meta" },
                new List<object> { ""             , "version" , "string" , "1.0" },
                new List<object> { ""             , ""        , ""       , ""      , "end.meta" },

                // Config stats - medium config bottom right
                new List<object> { ""             , ""        , ""       , ""      , ""          , "start.stats" },
                new List<object> { ""             , ""        , ""       , ""      , ""          , ""             , "metric" , "string" , "cpu"  , "ram"  , "disk" },
                new List<object> { ""             , ""        , ""       , ""      , ""          , ""             , "value"  , "float"  , "45.2" , "78.9" , "12.3" },
                new List<object> { ""             , ""        , ""       , ""      , ""          , ""             , ""       , ""       , ""     , ""     , ""       , "end.stats" }
            };
            var expectedPropertiesUsers = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("id", "int"),
                new ParsedPropertyInfo("name", "string"),
                new ParsedPropertyInfo("age", "int"),
                new ParsedPropertyInfo("salary", "float")
            };
            var expectedEntitiesUsers = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1", "Alice", "25", "50.5" },
                new List<string> { "2", "Bob", "30", "75.3" },
                new List<string> { "3", "Charlie", "35", "90.1" }
            };
            var expectedPropertiesSettings = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("key", "string"),
                new ParsedPropertyInfo("value", "string")
            };
            var expectedEntitiesSettings = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "theme", "dark" },
                new List<string> { "lang", "en" }
            };
            var expectedPropertiesMeta = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("version", "string")
            };
            var expectedEntitiesMeta = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1.0" }
            };
            var expectedPropertiesStats = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("metric", "string"),
                new ParsedPropertyInfo("value", "float")
            };
            var expectedEntitiesStats = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "cpu", "45.2" },
                new List<string> { "ram", "78.9" },
                new List<string> { "disk", "12.3" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(4, parsedSheets.Count);
            
            // Config users
            Assert.AreEqual("users", parsedSheets[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesUsers, parsedSheets[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesUsers, parsedSheets[0].Entities);
            
            // Config settings
            Assert.AreEqual("settings", parsedSheets[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesSettings, parsedSheets[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesSettings, parsedSheets[1].Entities);
            
            // Config meta
            Assert.AreEqual("meta", parsedSheets[2].Name);
            CollectionAssert.AreEqual(expectedPropertiesMeta, parsedSheets[2].Properties);
            CollectionAssert.AreEqual(expectedEntitiesMeta, parsedSheets[2].Entities);
            
            // Config stats
            Assert.AreEqual("stats", parsedSheets[3].Name);
            CollectionAssert.AreEqual(expectedPropertiesStats, parsedSheets[3].Properties);
            CollectionAssert.AreEqual(expectedEntitiesStats, parsedSheets[3].Entities);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ParseSheet_Edge_EmptySheet()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>();

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(0, parsedSheets.Count);
        }

        [Test]
        public void ParseSheet_Edge_SheetWithoutMarkers()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "random" , "data"   , "here" },
                new List<object> { "more"   , "data"   , "there" },
                new List<object> { "id"     , "int"    , "1"     , "2"    , "3" },
                new List<object> { "name"   , "string" , "Alice" , "Bob"  , "Charlie" },
                new List<object> { ""       , ""       , ""      , ""     , "" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(0, parsedSheets.Count);
        }

        [Test]
        public void ParseSheet_Edge_MinimalValidConfig()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "field" , "type" , "value" },
                new List<object> { ""          , ""      , ""     , ""        , "end.A" }
            };

            var expectedProperties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("field", "type")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "value" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(1, parsedSheets.Count);
            Assert.AreEqual("A", parsedSheets[0].Name);
            CollectionAssert.AreEqual(expectedProperties, parsedSheets[0].Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedSheets[0].Entities);
        }

        #endregion

        #region Negative Cases

        [Test]
        public void ParseSheet_Negative_EmptyStartName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start." },
                new List<object> { ""         , "field" , "type" , "value" },
                new List<object> { ""         , ""      , ""     , ""        , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("invalid name"));
        }

        [Test]
        public void ParseSheet_Negative_EmptyEndName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "field" , "type" , "value" },
                new List<object> { ""          , ""      , ""     , ""        , "end." }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("invalid name"));
        }

        [Test]
        public void ParseSheet_Negative_EndWithoutStart()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("no matching starts"));
        }

        [Test]
        public void ParseSheet_Negative_StartWithoutEnd()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "field" , "type" , "value" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("no matching ends"));
        }

        [Test]
        public void ParseSheet_Negative_NameMismatch()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "field" , "type" , "value" },
                new List<object> { ""          , ""      , ""     , ""        , "end.B" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("different name"));
        }

        [Test]
        public void ParseSheet_Negative_NestedConfigs()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "start.B" },
                new List<object> { ""          , ""         , "field" , "type" , "value" },
                new List<object> { ""          , ""         , ""      , ""     , ""        , "end.B" },
                new List<object> { ""          , ""         , ""      , ""     , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("< expected"));
        }

        [Test]
        public void ParseSheet_Negative_IntersectingConfigs()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "start.B" },
                new List<object> { ""          , ""         , ""      , "end.A" },
                new List<object> { ""          , ""         , "field" , "type"  , "value" },
                new List<object> { ""          , ""         , ""      , ""      , ""        , "end.B" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("with different name"));
        }

        [Test]
        public void ParseSheet_Negative_EndBeforeStartGeometrically()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { ""       , ""      , ""     , ""        , ""        , "end.A" },
                new List<object> { "" },
                new List<object> { "" },
                new List<object> { "start.A" },
                new List<object> { ""         , "field" , "type" , "value" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("no matching starts"));
        }

        [Test]
        public void ParseSheet_Negative_MultipleStartsSameName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "start.A" },
                new List<object> { ""          , ""         , "field" , "type" , "value" },
                new List<object> { ""          , ""         , ""      , ""     , ""        , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.IsTrue(result.Error.Contains("no matching ends"));
        }

        [Test]
        public void ParseSheet_Negative_ConfigWithoutDataBetweenStartAndEnd()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
        }

        #endregion

        #region Destructive Cases

        [Test]
        public void ParseSheet_Destructive_NullRowInMiddleOfSheet()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                null,  // Null row
                new List<object> { ""          , "field" , "type" , "value" },
                new List<object> { ""          , ""      , ""     , ""        , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void ParseSheet_Destructive_NullPropertyName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , null   , "type" , "value" },  // Null property name
                new List<object> { ""          , ""     , ""     , ""        , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk, "Expected error for null property name");
        }

        [Test]
        public void ParseSheet_Destructive_NullPropertyType()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""          , "field" , null   , "value" },  // Null property type
                new List<object> { ""          , ""      , ""     , ""        , "end.A" }
            };

            // Act
            var parsedSheets = new List<ParsedSheet>();
            var result = _sheetsParsing.ParseSheetNonAlloc(sheetName, sheetData, new HorizontalManySheetsOnOneParser(), parsedSheets);

            // Assert
            Assert.IsFalse(result.IsOk, "Expected error for null property type");
        }

        #endregion
    }
}