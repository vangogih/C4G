using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    public partial class ConfigsSerializerTests
    {
        public sealed class IntegerTests : ConfigsSerializerTests
        {
            [Test]
            public void Serialize_ExtremeIntegerValues()
            {
                // Arrange
                var name = "ExtremeValuesSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("MaxInt", "int"),
                    new ParsedProperty("MinInt", "int"),
                    new ParsedProperty("Zero", "int")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { int.MaxValue.ToString(), int.MinValue.ToString(), "0" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                string expectedOutput =
                    @"{
  ""ExtremeValuesSheet"": [
    {
      ""MaxInt"": 2147483647,
      ""MinInt"": -2147483648,
      ""Zero"": 0
    }
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_IntegerOverflow_LeadsToError()
            {
                // Arrange
                var name = "OverflowSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("TooBig", "int")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "999999999999999999999" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_InvalidSimpleTypeValue_LeadsToError()
            {
                // Arrange
                var name = "InvalidSimpleTypeSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Id", "int"),
                    new ParsedProperty("IsActive", "bool"),
                    new ParsedProperty("Score", "float")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "notanumber", "notabool", "notafloat" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsFalse(output.IsOk);
            }
        }
    }
}