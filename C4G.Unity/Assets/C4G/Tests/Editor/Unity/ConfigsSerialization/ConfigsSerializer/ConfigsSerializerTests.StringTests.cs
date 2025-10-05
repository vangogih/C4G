using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization.ConfigsSerializer
{
    public partial class ConfigsSerializerTests
    {
        public sealed class StringTests : ConfigsSerializerTests
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
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet });

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
  ""StringListSheet"": [
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
                Result<string, string> output = _configsSerializer.SerializeMultipleSheetsAsJsonObject(new List<ParsedSheet> { parsedSheet });

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }
    }
}