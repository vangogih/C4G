using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    [TestFixture]
    public partial class ConfigsSerializationTests
    {
        public sealed class FloatTests : ConfigsSerializationTests
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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

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
                Result<string, string> output = _configSerialization.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }
    }
}