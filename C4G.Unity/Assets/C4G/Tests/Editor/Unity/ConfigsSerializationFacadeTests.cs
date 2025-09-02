using System.Collections.Generic;
using C4G.Core;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class ConfigsSerializationFacadeTests
    {
        private ConfigsSerializationFacade _configSerializationFacade;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configSerializationFacade = new ConfigsSerializationFacade();
        }

        [Test]
        public void ConvertParsedSheetToJson_UsualCase()
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
      ""Id"": ""1"",
      ""Name"": ""Alice""
    },
    {
      ""Id"": ""2"",
      ""Name"": ""Bob""
    }
  ]
}";

            // Act
            Result<string, EC4GError> output = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void ConvertParsedSheetToJson_EmptyEntities()
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
            Result<string, EC4GError> output = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void ConvertParsedSheetToJson_EmptyProperties()
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
            Result<string, EC4GError> output = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedJson, output.Value);
        }

        [Test]
        public void ConvertParsedSheetToJson_MismatchedEntityData()
        {
            // Arrange
            var name = "MismatchedDataSheet";
            var properties = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Name", "string")
            };
            var entities = new List<List<string>>
            {
                new List<string> { "1" }, // Missing "Name"
                new List<string> { "2", "Bob", "ExtraData" } // Extra "ExtraData"
            };
            var parsedSheet = new ParsedSheet(name, properties, entities);

            // Act
            Result<string, EC4GError> result = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.CS_ParsedSheetMismatchedEntitiesCount, result.Error);
        }

        [Test]
        public void ConvertParsedSheetToJson_NullName_ThrowsException()
        {
            // Arrange
            var parsedSheet = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());

            // Act
            Result<string, EC4GError> result = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.CS_ParsedSheetNameNullOrEmpty, result.Error);
        }

        [Test]
        public void ConvertParsedSheetToJson_NullProperties_ThrowsException()
        {
            // Arrange
            var parsedSheet = new ParsedSheet("TestSheet", null, new List<List<string>>());

            // Act
            Result<string, EC4GError> result = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.CS_ParsedSheetPropertiesNull, result.Error);
        }

        [Test]
        public void ConvertParsedSheetToJson_NullEntities_ThrowsException()
        {
            // Arrange
            var parsedSheet = new ParsedSheet("TestSheet", new List<ParsedPropertyInfo>(), null);

            // Act
            Result<string, EC4GError> result = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsFalse(result.IsOk);
            Assert.AreEqual(EC4GError.CS_ParsedSheetEntitiesNull, result.Error);
        }
    }
}