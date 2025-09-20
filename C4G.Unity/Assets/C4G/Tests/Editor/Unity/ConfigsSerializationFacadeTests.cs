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
            Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

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
            Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

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
            Result<string, string> output = _configSerializationFacade.Serialize(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedJson, output.Value);
        }

        [Test]
        public void Serialize_WrongInputLeadsToError()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
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
            Result<string, string> nullNameResult = _configSerializationFacade.Serialize(parsedSheetWithNullName);
            Result<string, string> nullPropertiesResult = _configSerializationFacade.Serialize(parsedSheetWithNullProperties);
            Result<string, string> nullEntitiesResult = _configSerializationFacade.Serialize(parsedSheetWithNullEntities);
            Result<string, string> mismatchedDataResult = _configSerializationFacade.Serialize(parsedSheetWithMismatchedData);

            // Assert
            Assert.IsFalse(nullNameResult.IsOk);
            Assert.IsFalse(nullPropertiesResult.IsOk);
            Assert.IsFalse(nullEntitiesResult.IsOk);
            Assert.IsFalse(mismatchedDataResult.IsOk);
        }
    }
}