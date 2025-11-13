using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    public partial class ConfigsSerializerTests
    {
        public sealed class DoubleTests : ConfigsSerializerTests
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
  ""DoubleListSheet"": [
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
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }
    }
}