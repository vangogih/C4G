using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    [TestFixture]
    public partial class ConfigsSerializationFacadeTests
    {
        public sealed class GeneralTests : ConfigsSerializationFacadeTests
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
                Result<string, string> output = ConfigSerializationFacade.Serialize(parsedSheet);

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
                Result<string, string> output = ConfigSerializationFacade.Serialize(parsedSheet);

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
                Result<string, string> output = ConfigSerializationFacade.Serialize(parsedSheet);

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
                Result<string, string> nullNameResult = ConfigSerializationFacade.Serialize(parsedSheetWithNullName);
                Result<string, string> nullPropertiesResult =
                    ConfigSerializationFacade.Serialize(parsedSheetWithNullProperties);
                Result<string, string> nullEntitiesResult =
                    ConfigSerializationFacade.Serialize(parsedSheetWithNullEntities);
                Result<string, string> mismatchedDataResult =
                    ConfigSerializationFacade.Serialize(parsedSheetWithMismatchedData);

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
                Result<string, string> output = ConfigSerializationFacade.Serialize(parsedSheet);

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
                Result<string, string> output = ConfigSerializationFacade.Serialize(parsedSheet);

                // Assert
                Assert.IsTrue(output.IsOk);
                Assert.AreEqual(expectedOutput, output.Value);
            }
        }
    }
}