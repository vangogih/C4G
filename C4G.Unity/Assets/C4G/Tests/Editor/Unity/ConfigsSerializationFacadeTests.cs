using System.Collections.Generic;
using System.Linq;
using C4G.Core;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    [TestFixture]
    public class ConfigsSerializationFacadeTests
    {
        private ConfigsSerializationFacade _configSerializationFacade;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configSerializationFacade = new ConfigsSerializationFacade();
        }

        [TestFixture]
        public class GeneralTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_UsualCase()
            {
                // Arrange
                var name = "TestSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Name", "string")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "Alice" },
                    new List<string> { "2", "Bob" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""TestSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""Name"": ""Alice""
    },
    {
      ""Id"": 2,
      ""Name"": ""Bob""
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_EmptyEntities()
            {
                // Arrange
                var name = "EmptyEntitiesSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Name", "string")
                };
                var entities = new List<List<string>>();
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""EmptyEntitiesSheet"",
  ""entities"": []
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_EmptyProperties()
            {
                // Arrange
                var name = "EmptyPropertiesSheet";
                var properties = new List<ParsedPropertyInfo>();
                var entities = new List<List<string>> { new List<string>(), new List<string>() };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedJson =
                    @"{
  ""name"": ""EmptyPropertiesSheet"",
  ""entities"": [
    {},
    {}
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedJson, output.Value);
            }

            [Test]
            public void Serialize_WrongInput_LeadsToError()
            {
                // Arrange
                var parsedSheetWithNullName =
                    new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
                var parsedSheetWithNullProperties = new ParsedSheet("TestSheet", null, new List<List<string>>());
                var parsedSheetWithNullEntities = new ParsedSheet("TestSheet", new List<ParsedPropertyInfo>(), null);

                var parsedSheetWithMismatchedData = new ParsedSheet("MismatchedDataSheet",
                    new List<ParsedPropertyInfo>
                    {
                        new ParsedPropertyInfo("Id", "int"),
                        new ParsedPropertyInfo("Name", "string")
                    },
                    new List<List<string>>
                    {
                        new List<string> { "1" }, // Missing "Name"
                        new List<string> { "2", "Bob", "ExtraData" } // Extra "ExtraData"
                    });

                // Act
                Result<string, string> nullNameResult = _configSerializationFacade.Serialize(parsedSheetWithNullName);
                Result<string, string> nullPropertiesResult =
                    _configSerializationFacade.Serialize(parsedSheetWithNullProperties);
                Result<string, string> nullEntitiesResult =
                    _configSerializationFacade.Serialize(parsedSheetWithNullEntities);
                Result<string, string> mismatchedDataResult =
                    _configSerializationFacade.Serialize(parsedSheetWithMismatchedData);

                // Assert
                Assert.IsFalse(nullNameResult.IsOk);
                Assert.IsFalse(nullPropertiesResult.IsOk);
                Assert.IsFalse(nullEntitiesResult.IsOk);
                Assert.IsFalse(mismatchedDataResult.IsOk);
            }

            [Test]
            public void Serialize_UnknownPropertyType_LeadsToError()
            {
                // Arrange
                var name = "UnknownTypeSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("CustomProperty", "UnknownType")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "somevalue" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_MixedTypesWithLists()
            {
                // Arrange
                var name = "MixedTypesSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Name", "string"),
                    new ParsedPropertyInfo("IsActive", "bool"),
                    new ParsedPropertyInfo("Scores", "List<double>"),
                    new ParsedPropertyInfo("Tags", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "Alice", "true", "1.1,2.2,3.3", "admin,user,editor" },
                    new List<string> { "2", "Bob", "false", "4.4,5.5", "guest" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""MixedTypesSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""Name"": ""Alice"",
      ""IsActive"": true,
      ""Scores"": [
        1.1,
        2.2,
        3.3
      ],
      ""Tags"": [
        ""admin"",
        ""user"",
        ""editor""
      ]
    },
    {
      ""Id"": 2,
      ""Name"": ""Bob"",
      ""IsActive"": false,
      ""Scores"": [
        4.4,
        5.5
      ],
      ""Tags"": [
        ""guest""
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }

        [TestFixture]
        public class IntegerTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_ExtremeIntegerValues()
            {
                // Arrange
                var name = "ExtremeValuesSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("MaxInt", "int"),
                    new ParsedPropertyInfo("MinInt", "int"),
                    new ParsedPropertyInfo("Zero", "int")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { int.MaxValue.ToString(), int.MinValue.ToString(), "0" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""ExtremeValuesSheet"",
  ""entities"": [
    {
      ""MaxInt"": 2147483647,
      ""MinInt"": -2147483648,
      ""Zero"": 0
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_IntegerOverflow_LeadsToError()
            {
                // Arrange
                var name = "OverflowSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("TooBig", "int")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "999999999999999999999" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_InvalidSimpleTypeValue_LeadsToError()
            {
                // Arrange
                var name = "InvalidSimpleTypeSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("IsActive", "bool"),
                    new ParsedPropertyInfo("Score", "float")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "notanumber", "notabool", "notafloat" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }
        }

        [TestFixture]
        public class FloatTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_FloatSpecialValues()
            {
                // Arrange
                var name = "SpecialFloatSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Infinity", "float"),
                    new ParsedPropertyInfo("NegInfinity", "float"),
                    new ParsedPropertyInfo("NaN", "float"),
                    new ParsedPropertyInfo("Zero", "float")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "Infinity", "-Infinity", "NaN", "0.0" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""SpecialFloatSheet"",
  ""entities"": [
    {
      ""Infinity"": ""Infinity"",
      ""NegInfinity"": ""-Infinity"",
      ""NaN"": ""NaN"",
      ""Zero"": 0.0
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_ListOfFloats()
            {
                // Arrange
                var name = "FloatListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Name", "string"),
                    new ParsedPropertyInfo("Scores", "List<float>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "Player1", "1.5,2.7,3.9" },
                    new List<string> { "Player2", "10.1,20.2" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""FloatListSheet"",
  ""entities"": [
    {
      ""Name"": ""Player1"",
      ""Scores"": [
        1.5,
        2.7,
        3.9
      ]
    },
    {
      ""Name"": ""Player2"",
      ""Scores"": [
        10.1,
        20.2
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }

        [TestFixture]
        public class BooleanTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_BooleanVariations()
            {
                // Arrange
                var name = "BoolVariationsSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Bool1", "bool"),
                    new ParsedPropertyInfo("Bool2", "bool"),
                    new ParsedPropertyInfo("Bool3", "bool"),
                    new ParsedPropertyInfo("Bool4", "bool")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "True", "False", "TRUE", "FALSE" },
                    new List<string> { "true", "false", "tRuE", "fAlSe" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
            }

            [Test]
            public void Serialize_ListOfBooleans()
            {
                // Arrange
                var name = "BoolListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Flags", "List<bool>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "true,false,true" },
                    new List<string> { "2", "false,false" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""BoolListSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""Flags"": [
        true,
        false,
        true
      ]
    },
    {
      ""Id"": 2,
      ""Flags"": [
        false,
        false
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }

        [TestFixture]
        public class StringTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_StringWithSpecialCharacters()
            {
                // Arrange
                var name = "SpecialCharsSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("EmptyString", "string"),
                    new ParsedPropertyInfo("WithQuotes", "string"),
                    new ParsedPropertyInfo("WithNewlines", "string"),
                    new ParsedPropertyInfo("WithCommas", "string"),
                    new ParsedPropertyInfo("Unicode", "string")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "", "\"quoted\"", "line1\nline2", "has,comma", "emoji🎉测试" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                //Assert
                Assert.IsTrue(output.IsOk);
            }

            [Test]
            public void Serialize_ListOfStrings()
            {
                // Arrange
                var name = "StringListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Name", "string"),
                    new ParsedPropertyInfo("Tags", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "Alice", "tag1,tag2,tag3" },
                    new List<string> { "Bob", "admin,user" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""StringListSheet"",
  ""entities"": [
    {
      ""Name"": ""Alice"",
      ""Tags"": [
        ""tag1"",
        ""tag2"",
        ""tag3""
      ]
    },
    {
      ""Name"": ""Bob"",
      ""Tags"": [
        ""admin"",
        ""user""
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }

        [TestFixture]
        public class DoubleTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_ListOfDoubles()
            {
                // Arrange
                var name = "DoubleListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Name", "string"),
                    new ParsedPropertyInfo("Scores", "List<double>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "Player1", "1.5,2.7,3.9" },
                    new List<string> { "Player2", "10.1,20.2" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""DoubleListSheet"",
  ""entities"": [
    {
      ""Name"": ""Player1"",
      ""Scores"": [
        1.5,
        2.7,
        3.9
      ]
    },
    {
      ""Name"": ""Player2"",
      ""Scores"": [
        10.1,
        20.2
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }

        [TestFixture]
        public class ListTests : ConfigsSerializationFacadeTests
        {
            [Test]
            public void Serialize_ListOfIntegers()
            {
                // Arrange
                var name = "ListTestSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Numbers", "List<int>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "10,20,30" },
                    new List<string> { "2", "100,200" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""ListTestSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""Numbers"": [
        10,
        20,
        30
      ]
    },
    {
      ""Id"": 2,
      ""Numbers"": [
        100,
        200
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_EmptyList()
            {
                // Arrange
                var name = "EmptyListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("EmptyNumbers", "List<int>"),
                    new ParsedPropertyInfo("EmptyTags", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "", "" },
                    new List<string> { "2", "  ", "   " }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""EmptyListSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""EmptyNumbers"": [],
      ""EmptyTags"": []
    },
    {
      ""Id"": 2,
      ""EmptyNumbers"": [],
      ""EmptyTags"": []
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_ListWithWhitespaceElements()
            {
                // Arrange
                var name = "WhitespaceListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Items", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "  item1  ,  item2  ,  item3  " }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""WhitespaceListSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""Items"": [
        ""item1"",
        ""item2"",
        ""item3""
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_ListWithSingleElement()
            {
                // Arrange
                var name = "SingleElementSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("SingleItem", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "onlyitem" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                string expectedOutput =
                    @"{
  ""name"": ""SingleElementSheet"",
  ""entities"": [
    {
      ""Id"": 1,
      ""SingleItem"": [
        ""onlyitem""
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_InvalidListElementType_LeadsToError()
            {
                // Arrange
                var name = "InvalidListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("UnknownList", "List<UnknownType>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "value1,value2" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_InvalidListElementValue_LeadsToError()
            {
                // Arrange
                var name = "InvalidElementSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Numbers", "List<int>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "10,notanumber,30" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_ListWithEmptyElements_LeadsToError()
            {
                // Arrange
                var name = "EmptyElementsSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Items", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "item1,,item3" },
                    new List<string> { "2", "item1, ,item3" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_ListWithOnlyCommas_LeadsToError()
            {
                // Arrange
                var name = "OnlyCommasSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Items", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", ",,," }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_CaseInsensitiveListType_LeadsToError()
            {
                // Arrange
                var name = "CaseTestSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Items1", "list<int>"), // lowercase
                    new ParsedPropertyInfo("Items2", "LIST<INT>"), // uppercase  
                    new ParsedPropertyInfo("Items3", "List<String>") // mixed case
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1,2,3", "4,5,6", "a,b,c" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_MalformedListType_LeadsToError()
            {
                // Arrange
                var name = "MalformedSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("BadList1", "List<>"),
                    new ParsedPropertyInfo("BadList2", "List<int"),
                    new ParsedPropertyInfo("BadList3", "Listint>"),
                    new ParsedPropertyInfo("BadList4", "List<int,string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1,2", "3,4", "5,6", "7,8" }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_VeryLargeLists()
            {
                // Arrange
                var name = "LargeListSheet";
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("LargeList", "List<int>")
                };

                var largeList = string.Join(",", Enumerable.Range(1, 10000));
                var entities = new List<List<string>>
                {
                    new List<string> { "1", largeList }
                };
                var parsedSheet = new ParsedSheet(name, properties, entities);

                // Act
                Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
            }
        }
    }
}