using System;
using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NSubstitute;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    public partial class ConfigsSerializerTests
    {
        public sealed class AliasTests : ConfigsSerializerTests
        {
            [Test]
            public void Serialize_PrimitiveTypeAlias()
            {
                // Arrange
                IC4GTypeParser parser = CreateParser("100", 100);
                _parsersByName.Add("Health", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Health", "Health")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "100" }
                };
                var parsedSheet = new ParsedSheet("Character", properties, entities);

                string expectedOutput =
                    @"{
  ""Character"": [
    {
      ""Health"": 100
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_StringAlias()
            {
                // Arrange
                IC4GTypeParser parser = CreateParser("John", "John");
                _parsersByName.Add("PlayerName", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Name", "PlayerName")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "John" }
                };
                var parsedSheet = new ParsedSheet("Player", properties, entities);

                string expectedOutput =
                    @"{
  ""Player"": [
    {
      ""Name"": ""John""
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_EnumAlias()
            {
                // Arrange
                IC4GTypeParser parser = CreateParser("Rare", TestRarity.Rare);
                _parsersByName.Add("Rarity", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("ItemRarity", "Rarity")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "Rare" }
                };
                var parsedSheet = new ParsedSheet("Item", properties, entities);

                string expectedOutput =
                    @"{
  ""Item"": [
    {
      ""ItemRarity"": 1
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_ListAlias()
            {
                // Arrange
                IC4GTypeParser parser = CreateParser("1,2,3", new List<object> { 1, 2, 3 });
                _parsersByName.Add("IntList", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Values", "IntList")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1,2,3" }
                };
                var parsedSheet = new ParsedSheet("Container", properties, entities);

                string expectedOutput =
                    @"{
  ""Container"": [
    {
      ""Values"": [
        1,
        2,
        3
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_ArrayAlias()
            {
                // Arrange
                IC4GTypeParser parser = CreateParser("4,5,6", new object[] { 4, 5, 6 });
                _parsersByName.Add("IntArray", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Scores", "IntArray")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "4,5,6" }
                };
                var parsedSheet = new ParsedSheet("GameData", properties, entities);

                string expectedOutput =
                    @"{
  ""GameData"": [
    {
      ""Scores"": [
        4,
        5,
        6
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_DictionaryAlias()
            {
                // Arrange
                IC4GTypeParser parser = CreateParser("1:one,2:two",
                    new Dictionary<object, object> { { 1, "one" }, { 2, "two" } });
                _parsersByName.Add("IntToStringMap", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Mapping", "IntToStringMap")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1:one,2:two" }
                };
                var parsedSheet = new ParsedSheet("DataMapping", properties, entities);

                string expectedOutput =
                    @"{
  ""DataMapping"": [
    {
      ""Mapping"": {
        ""1"": ""one"",
        ""2"": ""two""
      }
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_MixedAliasesAndRegularTypes()
            {
                // Arrange
                IC4GTypeParser healthParser = CreateParser("150", 150);
                IC4GTypeParser playerNameParser = CreateParser("Alice", "Alice");
                _parsersByName.Add("Health", healthParser);
                _parsersByName.Add("PlayerName", playerNameParser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Id", "int"),
                    new ParsedPropertyInfo("Health", "Health"),
                    new ParsedPropertyInfo("Name", "PlayerName")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "150", "Alice" }
                };
                var parsedSheet = new ParsedSheet("Character", properties, entities);

                string expectedOutput =
                    @"{
  ""Character"": [
    {
      ""Id"": 1,
      ""Health"": 150,
      ""Name"": ""Alice""
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_MultipleEntitiesWithAlias()
            {
                // Arrange
                var parser = Substitute.For<IC4GTypeParser>();
                parser.Parse("100").Returns(Result<object, string>.FromValue((object)100));
                parser.Parse("200").Returns(Result<object, string>.FromValue((object)200));
                parser.Parse("300").Returns(Result<object, string>.FromValue((object)300));

                _parsersByName.Add("Health", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Health", "Health")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "100" },
                    new List<string> { "200" },
                    new List<string> { "300" }
                };
                var parsedSheet = new ParsedSheet("Character", properties, entities);

                string expectedOutput =
                    @"{
  ""Character"": [
    {
      ""Health"": 100
    },
    {
      ""Health"": 200
    },
    {
      ""Health"": 300
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_MultipleAliasesInOneEntity()
            {
                // Arrange
                IC4GTypeParser healthParser = CreateParser("75", 75);
                IC4GTypeParser rarityParser = CreateParser("Epic", TestRarity.Epic);
                IC4GTypeParser intListParser = CreateParser("10,20,30", new List<object> { 10, 20, 30 });
                _parsersByName.Add("Health", healthParser);
                _parsersByName.Add("Rarity", rarityParser);
                _parsersByName.Add("IntList", intListParser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Health", "Health"),
                    new ParsedPropertyInfo("ItemRarity", "Rarity"),
                    new ParsedPropertyInfo("Stats", "IntList")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "75", "Epic", "10,20,30" }
                };
                var parsedSheet = new ParsedSheet("ComplexItem", properties, entities);

                string expectedOutput =
                    @"{
  ""ComplexItem"": [
    {
      ""Health"": 75,
      ""ItemRarity"": 2,
      ""Stats"": [
        10,
        20,
        30
      ]
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_NonExistentAlias_ReturnsError()
            {
                // Arrange
                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Value", "UnknownAlias")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "someValue" }
                };
                var parsedSheet = new ParsedSheet("TestSheet", properties, entities);

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsFalse(output.IsOk);
                Assert.That(output.Error, Does.Contain("Cannot parse property with type 'UnknownAlias'"));
            }

            [Test]
            public void Serialize_AliasWithNullParser_ThrowsNre()
            {
                // Arrange
                _parsersByName.Add("NullParserAlias", null);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Value", "NullParserAlias")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "someValue" }
                };
                var parsedSheet = new ParsedSheet("TestSheet", properties, entities);

                // Act & Assert
                Assert.Throws<NullReferenceException>(() => _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName));
            }

            [Test]
            public void Serialize_AliasParserReturnsError()
            {
                // Arrange
                var parser = Substitute.For<IC4GTypeParser>();
                parser.Parse(Arg.Any<string>()).Returns(Result<object, string>.FromError("Custom parse error"));

                _parsersByName.Add("FailingAlias", parser);

                var properties = new List<ParsedPropertyInfo>
                {
                    new ParsedPropertyInfo("Value", "FailingAlias")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "someValue" }
                };
                var parsedSheet = new ParsedSheet("TestSheet", properties, entities);

                // Act
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsFalse(output.IsOk);
                Assert.That(output.Error, Does.Contain("Custom parse error"));
            }

            private static IC4GTypeParser CreateParser(string inputValue, object outputValue)
            {
                var parser = Substitute.For<IC4GTypeParser>();
                parser.Parse(inputValue).Returns(Result<object, string>.FromValue(outputValue));
                return parser;
            }
        }

        public enum TestRarity
        {
            // ReSharper disable UnusedMember.Global
            Common = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3
            // ReSharper restore UnusedMember.Global
        }
    }
}