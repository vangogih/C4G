using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.SheetsParsing
{
    [TestFixture]
    public class SubClassSheetParserTests
    {
        private Core.SheetsParsing.SheetsParsing _sheetsParsing;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sheetsParsing = new Core.SheetsParsing.SheetsParsing();
        }

        [Test]
        public void ParseSheet_SubClass_BasicCase()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Address.Street", "Address.City", "Name" },
                new List<object> { "int", "string", "string", "string" },
                new List<object> { "1", "Main St", "Springfield", "Alice" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Street", "string") { SubTypeIndex = 0 },
                new ParsedPropertyInfo("City", "string") { SubTypeIndex = 0 },
                new ParsedPropertyInfo("Name", "string")
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];

            Assert.AreEqual(1, parsedConfig.SubTypes.Count);
            Assert.AreEqual("Address", parsedConfig.SubTypes[0]);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
        }

        [Test]
        public void ParseSheet_SubClass_MultipleSubTypes()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Address.Street", "Phone.Number", "Name" },
                new List<object> { "int", "string", "string", "string" },
                new List<object> { "1", "Main St", "555-1234", "Alice" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Street", "string") { SubTypeIndex = 0 },
                new ParsedPropertyInfo("Number", "string") { SubTypeIndex = 1 },
                new ParsedPropertyInfo("Name", "string")
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];

            Assert.AreEqual(2, parsedConfig.SubTypes.Count);
            Assert.AreEqual("Address", parsedConfig.SubTypes[0]);
            Assert.AreEqual("Phone", parsedConfig.SubTypes[1]);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
        }

        [Test]
        public void ParseSheet_SubClass_AllPropertiesAreSubType()
        {
            // Arrange
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Address.Street", "Address.City" },
                new List<object> { "string", "string" },
                new List<object> { "Main St", "Springfield" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("Street", "string") { SubTypeIndex = 0 },
                new ParsedPropertyInfo("City", "string") { SubTypeIndex = 0 }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];

            Assert.AreEqual(1, parsedConfig.SubTypes.Count);
            Assert.AreEqual("Address", parsedConfig.SubTypes[0]);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
        }

        [Test]
        public void ParseSheet_SubClass_SameShortNameInDifferentSubTypes()
        {
            // Arrange: Address.Name and Phone.Name — same short name, different sub-types
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Address.Name", "Phone.Name" },
                new List<object> { "int", "string", "string" },
                new List<object> { "1", "Maple Street", "John" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Name", "string") { SubTypeIndex = 0 },
                new ParsedPropertyInfo("Name", "string") { SubTypeIndex = 1 }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];

            Assert.AreEqual(2, parsedConfig.SubTypes.Count);
            Assert.AreEqual("Address", parsedConfig.SubTypes[0]);
            Assert.AreEqual("Phone", parsedConfig.SubTypes[1]);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
        }

        [Test]
        public void ParseSheet_SubClass_InterleavedProperties()
        {
            // Arrange: root and sub-type properties interleaved — root | sub | root | sub
            string sheetName = "TestSheet";
            var sheetData = new List<IList<object>>
            {
                new List<object> { "Id", "Address.Street", "Name", "Address.City" },
                new List<object> { "int", "string", "string", "string" },
                new List<object> { "1", "Main St", "Alice", "Springfield" }
            };

            var expectedProperties = new ParsedPropertyInfo[]
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("Street", "string") { SubTypeIndex = 0 },
                new ParsedPropertyInfo("Name", "string"),
                new ParsedPropertyInfo("City", "string") { SubTypeIndex = 0 }
            };
            var expectedEntities = new List<IReadOnlyCollection<string>>
            {
                new List<string> { "1", "Main St", "Alice", "Springfield" }
            };

            // Act
            var parsedConfigs = new List<ParsedConfig>();
            var result = _sheetsParsing.ParseSheetToList(sheetName, sheetData, new VerticalSheetParser(), parsedConfigs);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, parsedConfigs.Count);
            var parsedConfig = parsedConfigs[0];

            Assert.AreEqual(1, parsedConfig.SubTypes.Count);
            Assert.AreEqual("Address", parsedConfig.SubTypes[0]);
            CollectionAssert.AreEqual(expectedProperties, parsedConfig.Properties);
            CollectionAssert.AreEqual(expectedEntities, parsedConfig.Entities);
        }
    }
}
