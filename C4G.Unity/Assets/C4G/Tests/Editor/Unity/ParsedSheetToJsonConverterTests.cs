using System;
using System.Collections.Generic;
using C4G.Editor.Core;
using C4G.Editor.Unity;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class ParsedSheetToJsonConverterTests
    {
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

            string expectedJson =
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
            string resultJson = ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

            // Assert
            Assert.AreEqual(expectedJson, resultJson);
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

            string expectedJson =
@"{
  ""name"": ""EmptyEntitiesSheet"",
  ""entities"": []
}";

            // Act
            string resultJson = ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

            // Assert
            Assert.AreEqual(expectedJson, resultJson);
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
            string resultJson = ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

            // Assert
            Assert.AreEqual(expectedJson, resultJson);
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

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet));
        }

        [Test]
        public void ConvertParsedSheetToJson_NullInput_ThrowsException()
        {
            // Act & Assert
            var ex = Assert.Throws<NullReferenceException>(() => ParsedSheetToJsonConverter.ConvertParsedSheetToJson(null));
            Assert.AreEqual("Parameter 'parsedSheet' is null", ex.Message);
        }

        [Test]
        public void ConvertParsedSheetToJson_NullName_ThrowsException()
        {
            // Arrange
            var parsedSheet = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());

            // Act & Assert
            var ex = Assert.Throws<NullReferenceException>(() => ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet));
            Assert.AreEqual("Property 'parsedSheet.Name' is null or empty", ex.Message);
        }

        [Test]
        public void ConvertParsedSheetToJson_NullProperties_ThrowsException()
        {
            // Arrange
            var parsedSheet = new ParsedSheet("TestSheet", null, new List<List<string>>());

            // Act & Assert
            var ex = Assert.Throws<NullReferenceException>(() => ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet));
            Assert.AreEqual("Property 'parsedSheet.Properties' is null", ex.Message);
        }

        [Test]
        public void ConvertParsedSheetToJson_NullEntities_ThrowsException()
        {
            // Arrange
            var parsedSheet = new ParsedSheet("TestSheet", new List<ParsedPropertyInfo>(), null);

            // Act & Assert
            var ex = Assert.Throws<NullReferenceException>(() => ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet));
            Assert.AreEqual("Property 'parsedSheet.Entities' is null", ex.Message);
        }
    }
}
