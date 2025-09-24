using System.Collections.Generic;
using System.Linq;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public partial class ConfigsSerializationTests
    {
        [TestFixture]
        public sealed class ListTests : ConfigsSerializationTests
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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
            }
        }
    }
}