using System.Collections.Generic;
using C4G.Core.SheetsParsing._1_PropertiesHierarchyTraversal;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    public partial class ConfigsSerializerTests
    {
        public sealed class GeneralTests : ConfigsSerializerTests
        {
            [Test]
            public void Serialize_UsualCase()
            {
                // Arrange
                var name = "TestSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Id", "int"),
                    new ParsedProperty("Name", "string")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "Alice" },
                    new List<string> { "2", "Bob" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                string expectedOutput =
                    @"{
  ""TestSheet"": [
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
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_EmptyEntities()
            {
                // Arrange
                var name = "EmptyEntitiesSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Id", "int"),
                    new ParsedProperty("Name", "string")
                };
                var entities = new List<List<string>>();
                var parsedConfig = new ParsedConfig(name, properties, entities);

                string expectedOutput =
                    @"{
  ""EmptyEntitiesSheet"": []
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }

            [Test]
            public void Serialize_EmptyProperties()
            {
                // Arrange
                var name = "EmptyPropertiesSheet";
                var properties = new List<ParsedProperty>();
                var entities = new List<List<string>> { new List<string>(), new List<string>() };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                string expectedJson =
                    @"{
  ""EmptyPropertiesSheet"": [
    {},
    {}
  ]
}";

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedJson, output.Value);
            }

            [Test]
            public void Serialize_WrongInput_LeadsToError()
            {
                // Arrange
                var parsedConfigWithNullName =
                    new ParsedConfig(null, new List<ParsedProperty>(), new List<List<string>>());
                var parsedConfigWithNullProperties = new ParsedConfig("TestSheet", null, new List<List<string>>());
                var parsedConfigWithNullEntities = new ParsedConfig("TestSheet", new List<ParsedProperty>(), null);

                var parsedConfigWithMismatchedData = new ParsedConfig("MismatchedDataSheet",
                    new List<ParsedProperty>
                    {
                        new ParsedProperty("Id", "int"),
                        new ParsedProperty("Name", "string")
                    },
                    new List<List<string>>
                    {
                        new List<string> { "1" }, // Missing "Name"
                        new List<string> { "2", "Bob", "ExtraData" } // Extra "ExtraData"
                    });

                // Act
                Result<string, string> nullNameResult = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfigWithNullName }, _parsersByName);
                Result<string, string> nullPropertiesResult =
                    _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfigWithNullProperties }, _parsersByName);
                Result<string, string> nullEntitiesResult =
                    _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfigWithNullEntities }, _parsersByName);
                Result<string, string> mismatchedDataResult =
                    _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfigWithMismatchedData }, _parsersByName);

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
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Id", "int"),
                    new ParsedProperty("CustomProperty", "UnknownType")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "somevalue" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                // Act
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsFalse(output.IsOk);
            }

            [Test]
            public void Serialize_MixedTypesWithLists()
            {
                // Arrange
                var name = "MixedTypesSheet";
                var properties = new List<ParsedProperty>
                {
                    new ParsedProperty("Id", "int"),
                    new ParsedProperty("Name", "string"),
                    new ParsedProperty("IsActive", "bool"),
                    new ParsedProperty("Scores", "List<double>"),
                    new ParsedProperty("Tags", "List<string>")
                };
                var entities = new List<List<string>>
                {
                    new List<string> { "1", "Alice", "true", "1.1,2.2,3.3", "admin,user,editor" },
                    new List<string> { "2", "Bob", "false", "4.4,5.5", "guest" }
                };
                var parsedConfig = new ParsedConfig(name, properties, entities);

                string expectedOutput =
                    @"{
  ""MixedTypesSheet"": [
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
                Result<string, string> output = _configsSerializer.SerializeParsedConfigsAsJsonObject(new List<ParsedConfig> { parsedConfig }, _parsersByName);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }
    }
}