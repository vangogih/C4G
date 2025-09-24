using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using C4G.Editor;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class CodeGenerationTests
    {
        private CodeGeneration _codeGeneration;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _codeGeneration = new CodeGeneration();
        }

        [Test]
        public void GenerateDTOClass_UsualCase()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class ClassName
{{
    public int Id {{ get; set; }}
    public int BaseHp {{ get; set; }}
}}
";
            var className = "ClassName";
            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("BaseHp", "int")
            };
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(className, propertyInfos, entities);

            // Act
            Result<string, string> output = _codeGeneration.GenerateDTOClass(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_WrongInputLeadsToError()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithEmptyName = new ParsedSheet(string.Empty, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithNullProps = new ParsedSheet("ClassName", null, new List<List<string>>());
            var parsedSheetWithNullEntities = new ParsedSheet("ClassName", new List<ParsedPropertyInfo>(), null);

            // Act
            Result<string, string> nullNameOutput = _codeGeneration.GenerateDTOClass(parsedSheetWithNullName);
            Result<string, string> emptyNameOutput = _codeGeneration.GenerateDTOClass(parsedSheetWithEmptyName);
            Result<string, string> nullPropsOutput = _codeGeneration.GenerateDTOClass(parsedSheetWithNullProps);
            Result<string, string> nullEntitiesOutput = _codeGeneration.GenerateDTOClass(parsedSheetWithNullEntities);

            // Assert
            Assert.IsFalse(nullNameOutput.IsOk);
            Assert.IsFalse(emptyNameOutput.IsOk);
            Assert.IsFalse(nullPropsOutput.IsOk);
            Assert.IsFalse(nullEntitiesOutput.IsOk);
        }

        [Test]
        public void GenerateWrapperClass_UsualCase()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class ClassNameWrapper
{{
    public string Name {{ get; set; }}
    public List<ClassName> Entities {{ get; set; }} = new List<ClassName>();
}}
";
            var className = "ClassName";
            var propertyInfos = new List<ParsedPropertyInfo>();
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(className, propertyInfos, entities);

            // Act
            Result<string, string> output = _codeGeneration.GenerateWrapperClass(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateWrapperClass_WrongInputLeadsToError()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithEmptyName = new ParsedSheet(string.Empty, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithNullProps = new ParsedSheet("ClassName", null, new List<List<string>>());
            var parsedSheetWithNullEntities = new ParsedSheet("ClassName", new List<ParsedPropertyInfo>(), null);

            // Act
            Result<string, string> nullNameOutput = _codeGeneration.GenerateWrapperClass(parsedSheetWithNullName);
            Result<string, string> emptyNameOutput = _codeGeneration.GenerateWrapperClass(parsedSheetWithEmptyName);
            Result<string, string> nullPropsOutput = _codeGeneration.GenerateWrapperClass(parsedSheetWithNullProps);
            Result<string, string> nullEntitiesOutput = _codeGeneration.GenerateWrapperClass(parsedSheetWithNullEntities);

            // Assert
            Assert.IsFalse(nullNameOutput.IsOk);
            Assert.IsFalse(emptyNameOutput.IsOk);
            Assert.IsFalse(nullPropsOutput.IsOk);
            Assert.IsFalse(nullEntitiesOutput.IsOk);
        }
    }
}