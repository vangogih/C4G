using System.Collections.Generic;
using C4G.Core;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using C4G.Editor;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public class CodeGenerationFacadeTests
    {
        private CodeGenerationFacade _codeGenerationFacade;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _codeGenerationFacade = new CodeGenerationFacade();
        }

        [Test]
        public void GenerateDTOClass_UsualCase()
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
            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Id", "int"),
                new ParsedPropertyInfo("BaseHp", "int")
            };
            var entities = new List<List<string>>();
            var parsedSheet = new ParsedSheet(className, propertyInfos, entities);

            // Act
            Result<string, EC4GError> output = _codeGenerationFacade.GenerateDTOClass(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_WrongInputLeadsToErrorAsResult()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithEmptyName = new ParsedSheet(string.Empty, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithNullProps = new ParsedSheet("ClassName", null, new List<List<string>>());
            var parsedSheetWithNullEntities = new ParsedSheet("ClassName", new List<ParsedPropertyInfo>(), null);

            // Act
            Result<string, EC4GError> nullNameOutput = _codeGenerationFacade.GenerateDTOClass(parsedSheetWithNullName);
            Result<string, EC4GError> emptyNameOutput = _codeGenerationFacade.GenerateDTOClass(parsedSheetWithEmptyName);
            Result<string, EC4GError> nullPropsOutput = _codeGenerationFacade.GenerateDTOClass(parsedSheetWithNullProps);
            Result<string, EC4GError> nullEntitiesOutput = _codeGenerationFacade.GenerateDTOClass(parsedSheetWithNullEntities);

            // Assert
            Assert.IsFalse(nullNameOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetNameNullOrEmpty, nullNameOutput.Error);

            Assert.IsFalse(emptyNameOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetNameNullOrEmpty, emptyNameOutput.Error);

            Assert.IsFalse(nullPropsOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetPropertiesNull, nullPropsOutput.Error);

            Assert.IsFalse(nullEntitiesOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetEntitiesNull, nullEntitiesOutput.Error);
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
            Result<string, EC4GError> output = _codeGenerationFacade.GenerateWrapperClass(parsedSheet);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateWrapperClass_WrongInputLeadsToErrorAsResult()
        {
            // Arrange
            var parsedSheetWithNullName = new ParsedSheet(null, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithEmptyName = new ParsedSheet(string.Empty, new List<ParsedPropertyInfo>(), new List<List<string>>());
            var parsedSheetWithNullProps = new ParsedSheet("ClassName", null, new List<List<string>>());
            var parsedSheetWithNullEntities = new ParsedSheet("ClassName", new List<ParsedPropertyInfo>(), null);

            // Act
            Result<string, EC4GError> nullNameOutput = _codeGenerationFacade.GenerateWrapperClass(parsedSheetWithNullName);
            Result<string, EC4GError> emptyNameOutput = _codeGenerationFacade.GenerateWrapperClass(parsedSheetWithEmptyName);
            Result<string, EC4GError> nullPropsOutput = _codeGenerationFacade.GenerateWrapperClass(parsedSheetWithNullProps);
            Result<string, EC4GError> nullEntitiesOutput = _codeGenerationFacade.GenerateWrapperClass(parsedSheetWithNullEntities);

            // Assert
            Assert.IsFalse(nullNameOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetNameNullOrEmpty, nullNameOutput.Error);

            Assert.IsFalse(emptyNameOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetNameNullOrEmpty, emptyNameOutput.Error);

            Assert.IsFalse(nullPropsOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetPropertiesNull, nullPropsOutput.Error);

            Assert.IsFalse(nullEntitiesOutput.IsOk);
            Assert.AreEqual(EC4GError.CG_ParsedSheetEntitiesNull, nullEntitiesOutput.Error);
        }
    }
}