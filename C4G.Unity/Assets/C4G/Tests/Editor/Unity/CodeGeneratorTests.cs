using System;
using System.Collections.Generic;
using C4G.Editor;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class CodeGeneratorTests
    {
        [Test]
        public void GenerateDTOClass_UsualCase()
        {
            // Arrange
            string expectedOutput =
@"public partial class ClassName
{
    public int Id { get; set; }
}
";
            var className = "ClassName";
            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int")
            };
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(className, propertyInfos, entities);

            // Act
            string output = CodeGenerator.GenerateDTOClass(parsedSheet);

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void GenerateDTOClass_PropertiesOrderMatters()
        {
            // Arrange
            string expectedOutput = 
@"public partial class ClassName
{
    public int Id { get; set; }
    public int BaseHp { get; set; }
}
";
            var className = "ClassName";
            var differentOrderPropertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("BaseHp", "int"),
                new ParsedPropertyInfo("Id", "int"),
            };
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(className, differentOrderPropertyInfos, entities);

            // Act
            string output = CodeGenerator.GenerateDTOClass(parsedSheet);

            // Assert
            Assert.AreNotEqual(expectedOutput, output);
        }

        [Test]
        public void GenerateDTOClass_ClassNameMatters()
        {
            // Arrange
            string expectedOutput = 
@"public partial class ClassName
{
}
";
            var differentClassName = "DifferentClassName";
            var propertyInfos = new List<ParsedPropertyInfo>();
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(differentClassName, propertyInfos, entities);

            // Act
            string output = CodeGenerator.GenerateDTOClass(parsedSheet);

            // Assert
            Assert.AreNotEqual(expectedOutput, output);
        }

        [Test]
        public void GenerateDTOClass_NullInputLeadsToException()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithEmptyName = new ParsedSheet(string.Empty, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithNullProps = new ParsedSheet("ClassName", null, new List<List<string>>());
            var parsedSheetWithNullEntities = new ParsedSheet("ClassName", new List<ParsedPropertyInfo>(), null);

            // Act
            var nullInputException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateDTOClass(null));
            var nullNameException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateDTOClass(parsedSheetWithNullName));
            var emptyNameException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateDTOClass(parsedSheetWithEmptyName));
            var nullPropsException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateDTOClass(parsedSheetWithNullProps));
            var nullEntitiesException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateDTOClass(parsedSheetWithNullEntities));

            // Assert
            Assert.AreEqual("Parameter 'parsedSheet' is null", nullInputException.Message);
            Assert.AreEqual("Property 'parsedSheet.Name' is null or empty", nullNameException.Message);
            Assert.AreEqual("Property 'parsedSheet.Name' is null or empty", emptyNameException.Message);
            Assert.AreEqual("Property 'parsedSheet.Properties' is null", nullPropsException.Message);
            Assert.AreEqual("Property 'parsedSheet.Entities' is null", nullEntitiesException.Message);
        }

        [Test]
        public void GenerateWrapperClass_UsualCase()
        {
            // Arrange
            string expectedOutput =
@"using System.Collections.Generic;

public partial class ClassNameWrapper
{
    public string Name { get; set; }
    public List<ClassName> Entities { get; set; } = new List<ClassName>();
}
";
            var className = "ClassName";
            var propertyInfos = new List<ParsedPropertyInfo>();
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(className, propertyInfos, entities);

            // Act
            string output = CodeGenerator.GenerateWrapperClass(parsedSheet);

            // Assert
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public void GenerateWrapperClass_ClassNameMatters()
        {
            // Arrange
            string expectedOutput =
@"using System.Collections.Generic;

public partial class ClassNameWrapper
{
    public string Name { get; set; }
    public List<ClassName> Entities { get; set; } = new List<ClassName>();
}
";
            var differentClassName = "DifferentClassName";
            var propertyInfos = new List<ParsedPropertyInfo>();
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(differentClassName, propertyInfos, entities);

            // Act
            string output = CodeGenerator.GenerateWrapperClass(parsedSheet);

            // Assert
            Assert.AreNotEqual(expectedOutput, output);
        }

        [Test]
        public void GenerateWrapperClass_NullInputLeadsToException()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithEmptyName = new ParsedSheet(string.Empty, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithNullProps = new ParsedSheet("ClassName", null, new List<List<string>>());
            var parsedSheetWithNullEntities = new ParsedSheet("ClassName", new List<ParsedPropertyInfo>(), null);

            // Act & Assert
            var nullInputException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateWrapperClass(null));
            var nullNameException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateWrapperClass(parsedSheetWithNullName));
            var emptyNameException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateWrapperClass(parsedSheetWithEmptyName));
            var nullPropsException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateWrapperClass(parsedSheetWithNullProps));
            var nullEntitiesException = Assert.Throws<NullReferenceException>(() => CodeGenerator.GenerateWrapperClass(parsedSheetWithNullEntities));
            Assert.AreEqual("Parameter 'parsedSheet' is null", nullInputException.Message);
            Assert.AreEqual("Property 'parsedSheet.Name' is null or empty", nullNameException.Message);
            Assert.AreEqual("Property 'parsedSheet.Name' is null or empty", emptyNameException.Message);
            Assert.AreEqual("Property 'parsedSheet.Properties' is null", nullPropsException.Message);
            Assert.AreEqual("Property 'parsedSheet.Entities' is null", nullEntitiesException.Message);
        }
    }
}