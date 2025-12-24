using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    public partial class ConfigsSerializerTests
    {
        public sealed class BooleanTests : ConfigsSerializerTests
        {
            [Test]
            public void Serialize_BooleanVariations()
            {
                // Arrange
                var name = "BoolVariationsSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Bool1", "bool"),
                    new ParsedProperty("Bool2", "bool"),
                    new ParsedProperty("Bool3", "bool"),
                    new ParsedProperty("Bool4", "bool")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "True", "False", "TRUE", "FALSE" },
                    new List<string> { "true", "false", "tRuE", "fAlSe" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
            }

            [Test]
            public void Serialize_ListOfBooleans()
            {
                // Arrange
                var name = "BoolListSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Id", "int"),
                    new ParsedProperty("Flags", "List<bool>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "true,false,true" },
                    new List<string> { "2", "false,false" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                string expectedOutput =
                    @"{
  ""BoolListSheet"": [
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
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }
    }
}