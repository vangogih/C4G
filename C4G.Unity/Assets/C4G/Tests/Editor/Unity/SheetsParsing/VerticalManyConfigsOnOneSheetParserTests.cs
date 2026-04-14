using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
    [TestFixture]
    public class VerticalManyConfigsOnOneSheetParserTests
    {
        private Core.SheetsParsing.SheetsParsing _sheetsParsing;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsing = new Core.SheetsParsing.SheetsParsing();
        }

        [Test]
        public void ParseSheet_Positive_SingleValidConfig()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.users" },
                new List<object> { ""            , "id"     , "name"    , "age"   },
                new List<object> { ""            , "int"    , "string"  , "float" },
                new List<object> { ""            , "1"      , "Alex"    , "27.6"  },
                new List<object> { ""            , "123"    , "Dima"    , "12.1"  },
                new List<object> { ""            , "456"    , "Serghei" , "13.5"  },
                new List<object> { ""            , ""       , ""        , ""      , "end.users" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];
            Assert.AreEqual("users", parsedConfig.Name);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfig.Entities);
        }

        [Test]
        public void ParseSheet_Positive_OneValidConfigNextToAnotherValidConfig()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                // Config A                     Config B
                new List<object> { "start.A"  , ""       , ""      , ""   , "start.B" },
                new List<object> { ""         , "field1" , ""      , ""   , ""        , "id"     , "name"   },
                new List<object> { ""         , "type1"  , ""      , ""   , ""        , "int"    , "string" },
                new List<object> { ""         , "val1"   , ""      , ""   , ""        , "10"     , "X"      },
                new List<object> { ""         , "val2"   , ""      , ""   , ""        , "20"     , "Y"      },
                new List<object> { ""         , ""       , "end.A" , ""   , ""        , "30"     , "Z"      },
                new List<object> { ""         , ""       , ""      , ""   , ""        , ""       , ""       , "end.B" }
            };
            var expectedPropertiesA = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("field1", "type1")
            };
            var expectedEntitiesA = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "val1" },
                new List<string> { "val2" }
            };
            var expectedPropertiesB = new ParsedPropertyInfo[]
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(2, parsedConfigs.Count);
            Assert.AreEqual("A", parsedConfigs[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesA, parsedConfigs[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesA, parsedConfigs[0].Entities);
            Assert.AreEqual("B", parsedConfigs[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesB, parsedConfigs[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesB, parsedConfigs[1].Entities);
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
                new List<object> { ""        , "x"   },
                new List<object> { ""        , "int" },
                new List<object> { ""        , "1"   },
                new List<object> { ""        , "2"   },
                new List<object> { ""        , ""    , "end.A" },

                // Empty row
                new List<object> { "" },

                // Config B - below and to the right
                new List<object> { ""        , ""    , ""      , "start.B" },
                new List<object> { ""        , ""    , ""      , ""        , "y"      },
                new List<object> { ""        , ""    , ""      , ""        , "string" },
                new List<object> { ""        , ""    , ""      , ""        , "alpha"  },
                new List<object> { ""        , ""    , ""      , ""        , "beta"   },
                new List<object> { ""        , ""    , ""      , ""        , ""       , "end.B" }
            };
            var expectedPropertiesA = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("x", "int")
            };
            var expectedEntitiesA = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1" },
                new List<string> { "2" }
            };
            var expectedPropertiesB = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("y", "string")
            };
            var expectedEntitiesB = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "alpha" },
                new List<string> { "beta" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(2, parsedConfigs.Count);
            Assert.AreEqual("A", parsedConfigs[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesA, parsedConfigs[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesA, parsedConfigs[0].Entities);
            Assert.AreEqual("B", parsedConfigs[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesB, parsedConfigs[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesB, parsedConfigs[1].Entities);
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
                new List<object> { ""        , "id"  },
                new List<object> { ""        , "int" },
                new List<object> { ""        , "100" },
                new List<object> { ""        , ""    , "end.A" },

                // Empty row
                new List<object> { "" },

                // Config B - below and to the right
                new List<object> { ""        , ""    , ""       , "start.B" },
                new List<object> { ""        , ""    , ""       , ""        , "name"   , "active" },
                new List<object> { ""        , ""    , ""       , ""        , "string" , "bool"   },
                new List<object> { ""        , ""    , ""       , ""        , "John"   , "true"   },
                new List<object> { ""        , ""    , ""       , ""        , "Jane"   , "false"  },
                new List<object> { ""        , ""    , ""       , ""        , ""       , ""       , "end.B" }
            };
            var expectedPropertiesA = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("id", "int")
            };
            var expectedEntitiesA = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "100" }
            };
            var expectedPropertiesB = new ParsedPropertyInfo[]
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(2, parsedConfigs.Count);
            Assert.AreEqual("A", parsedConfigs[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesA, parsedConfigs[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesA, parsedConfigs[0].Entities);
            Assert.AreEqual("B", parsedConfigs[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesB, parsedConfigs[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesB, parsedConfigs[1].Entities);
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
                new List<object> { ""            , "id"     , "name"    , "age"    , "salary" },
                new List<object> { ""            , "int"    , "string"  , "int"    , "float"  },
                new List<object> { ""            , "1"      , "Alice"   , "25"     , "50.5"   },
                new List<object> { ""            , "2"      , "Bob"     , "30"     , "75.3"   },
                new List<object> { ""            , "3"      , "Charlie" , "35"     , "90.1"   },
                new List<object> { ""            , ""       , ""        , ""       , ""       , "end.users" },

                // Empty row
                new List<object> { "" },

                // Config settings - small config middle right
                new List<object> { ""            , ""       , ""        , ""       , ""       , ""             , "start.settings" },
                new List<object> { ""            , ""       , ""        , ""       , ""       , ""             , ""               , "key"    , "value"  },
                new List<object> { ""            , ""       , ""        , ""       , ""       , ""             , ""               , "string" , "string" },
                new List<object> { ""            , ""       , ""        , ""       , ""       , ""             , ""               , "theme"  , "dark"   },
                new List<object> { ""            , ""       , ""        , ""       , ""       , ""             , ""               , "lang"   , "en"     },
                new List<object> { ""            , ""       , ""        , ""       , ""       , ""             , ""               , ""       , ""       , "end.settings" },

                // Empty rows
                new List<object> { "" },
                new List<object> { "" },

                // Config meta - tiny config bottom left
                new List<object> { "start.meta" },
                new List<object> { ""           , "version" },
                new List<object> { ""           , "string"  },
                new List<object> { ""           , "1.0"     },
                new List<object> { ""           , ""        , "end.meta" },

                // Config stats - medium config bottom right
                new List<object> { ""           , ""        , ""         , "start.stats" },
                new List<object> { ""           , ""        , ""         , ""            , "metric" , "value" },
                new List<object> { ""           , ""        , ""         , ""            , "string" , "float" },
                new List<object> { ""           , ""        , ""         , ""            , "cpu"    , "45.2"  },
                new List<object> { ""           , ""        , ""         , ""            , "ram"    , "78.9"  },
                new List<object> { ""           , ""        , ""         , ""            , "disk"   , "12.3"  },
                new List<object> { ""           , ""        , ""         , ""            , ""       , ""      , "end.stats" }
            };
            var expectedPropertiesUsers = new ParsedPropertyInfo[]
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
            var expectedPropertiesSettings = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("key", "string"),
                new ParsedPropertyInfo("value", "string")
            };
            var expectedEntitiesSettings = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "theme", "dark" },
                new List<string> { "lang", "en" }
            };
            var expectedPropertiesMeta = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("version", "string")
            };
            var expectedEntitiesMeta = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1.0" }
            };
            var expectedPropertiesStats = new ParsedPropertyInfo[]
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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(4, parsedConfigs.Count);
            
            // Config users
            Assert.AreEqual("users", parsedConfigs[0].Name);
            CollectionAssert.AreEqual(expectedPropertiesUsers, parsedConfigs[0].Properties);
            CollectionAssert.AreEqual(expectedEntitiesUsers, parsedConfigs[0].Entities);
            
            // Config settings
            Assert.AreEqual("settings", parsedConfigs[1].Name);
            CollectionAssert.AreEqual(expectedPropertiesSettings, parsedConfigs[1].Properties);
            CollectionAssert.AreEqual(expectedEntitiesSettings, parsedConfigs[1].Entities);
            
            // Config meta
            Assert.AreEqual("meta", parsedConfigs[2].Name);
            CollectionAssert.AreEqual(expectedPropertiesMeta, parsedConfigs[2].Properties);
            CollectionAssert.AreEqual(expectedEntitiesMeta, parsedConfigs[2].Entities);
            
            // Config stats
            Assert.AreEqual("stats", parsedConfigs[3].Name);
            CollectionAssert.AreEqual(expectedPropertiesStats, parsedConfigs[3].Properties);
            CollectionAssert.AreEqual(expectedEntitiesStats, parsedConfigs[3].Entities);
        }

        [Test]
        public void ParseSheet_Edge_EmptySheet()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>();

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(0, parsedConfigs.Count);
        }

        [Test]
        public void ParseSheet_Edge_SheetWithoutMarkers()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "random" , "data"   , "here"  },
                new List<object> { "more"   , "data"   , "there" },
                new List<object> { "id"     , "int"    , "1"     , "2"    , "3" },
                new List<object> { "name"   , "string" , "Alice" , "Bob"  , "Charlie" },
                new List<object> { ""       , ""       , ""      , ""     , "" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(0, parsedConfigs.Count);
        }

        [Test]
        public void ParseSheet_Edge_MinimalValidConfig()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                new List<object> { ""        , "field" },
                new List<object> { ""        , "type"  },
                new List<object> { ""        , "value" },
                new List<object> { ""        , ""      , "end.A" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("field", "type")
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "value" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk, $"Expected success but got error: {result.Error}");
            Assert.AreEqual(1, parsedConfigs.Count);
            Assert.AreEqual("A", parsedConfigs[0].Name);
            CollectionAssert.AreEqual(expectedProperties, parsedConfigs[0].Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfigs[0].Entities);
        }

        [Test]
        public void ParseSheet_Negative_EmptyStartName()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start." },
                new List<object> { ""       , "field" },
                new List<object> { ""       , "type"  },
                new List<object> { ""       , "value" },
                new List<object> { ""       , ""      , "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { ""        , "field" },
                new List<object> { ""        , "type"  },
                new List<object> { ""        , "value" },
                new List<object> { ""        , ""      , "end." }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { ""        , "field" },
                new List<object> { ""        , "type"  },
                new List<object> { ""        , "value" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { ""        , "field" },
                new List<object> { ""        , "type"  },
                new List<object> { ""        , "value" },
                new List<object> { ""        , ""      , "end.B" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { "start.A" , "start.B" },
                new List<object> { ""        , ""        , "field" },
                new List<object> { ""        , ""        , "type"  },
                new List<object> { ""        , ""        , "value" },
                new List<object> { ""        , ""        , ""      , "end.B" },
                new List<object> { ""        , "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void ParseSheet_Negative_IntersectingConfigs()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" , "start.B" },
                new List<object> { ""        , ""        , "end.A" },
                new List<object> { ""        , ""        , ""      , "field" },
                new List<object> { ""        , ""        , ""      , "type"  },
                new List<object> { ""        , ""        , ""      , "value" },
                new List<object> { ""        , ""        , ""      , ""      , "end.B" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { ""        , ""       , ""      , ""       , "end.A" },
                new List<object> { "" },
                new List<object> { "" },
                new List<object> { "start.A" },
                new List<object> { ""        , "field" },
                new List<object> { ""        , "type"  },
                new List<object> { ""        , "value" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { "start.A" , "start.A" },
                new List<object> { ""        , ""        , "field" },
                new List<object> { ""        , ""        , "type"  },
                new List<object> { ""        , ""        , "value" },
                new List<object> { ""        , ""        , ""      , "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { "start.A" },
                new List<object> { "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void ParseSheet_Destructive_NullRowInMiddleOfSheet()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "start.A" },
                null,  // Null row
                new List<object> { ""        , "field" },
                new List<object> { ""        , "type"  },
                new List<object> { ""        , "value" },
                new List<object> { ""        , ""      , "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { ""        , null   },  // Null property name
                new List<object> { ""        , "type" },
                new List<object> { ""        , "value" },
                new List<object> { ""        , ""     , "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

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
                new List<object> { ""        , "field" },
                new List<object> { ""        , null   },  // Null property type
                new List<object> { ""        , "value" },
                new List<object> { ""        , ""      , "end.A" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalManyConfigsOnOneSheetParser(), parsedConfigs);

            // Assert
            Assert.IsFalse(result.IsOk, "Expected error for null property type");
        }
    }
}