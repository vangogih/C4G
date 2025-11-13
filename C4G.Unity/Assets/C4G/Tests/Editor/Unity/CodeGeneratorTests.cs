using System.Collections.Generic;
using C4G.Core.CodeGeneration;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NSubstitute;
using NUnit.Framework;
using System;

namespace C4G.Tests.Editor.Unity
{
    public class CodeGeneratorTests
    {
        private CodeGenerator _codeGenerator;
        private Dictionary<string, IC4GTypeParser> _parsersByName;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _codeGenerator = new CodeGenerator();
            _parsersByName = new Dictionary<string, IC4GTypeParser>();
        }

        [SetUp]
        public void SetUp()
        {
            _parsersByName.Clear();
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
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

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
            Result<string, string> nullNameOutput = _codeGenerator.GenerateDTOClass(parsedSheetWithNullName, _parsersByName);
            Result<string, string> emptyNameOutput = _codeGenerator.GenerateDTOClass(parsedSheetWithEmptyName, _parsersByName);
            Result<string, string> nullPropsOutput = _codeGenerator.GenerateDTOClass(parsedSheetWithNullProps, _parsersByName);
            Result<string, string> nullEntitiesOutput = _codeGenerator.GenerateDTOClass(parsedSheetWithNullEntities, _parsersByName);

            // Assert
            Assert.IsFalse(nullNameOutput.IsOk);
            Assert.IsFalse(emptyNameOutput.IsOk);
            Assert.IsFalse(nullPropsOutput.IsOk);
            Assert.IsFalse(nullEntitiesOutput.IsOk);
        }

        [Test]
        public void GenerateDTOClass_PrimitiveTypeAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Character
{{
    public System.Int32 Health {{ get; set; }}
}}
";

            IC4GTypeParser parser = CreateParserForType(typeof(int));
            _parsersByName.Add("Health", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Health", "Health")
            };
            var parsedSheet = new ParsedSheet("Character", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_EnumAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Item
{{
    public C4G.Tests.Editor.Unity.TestEnum TestEnum {{ get; set; }}
}}
";
            IC4GTypeParser parser = CreateParserForType(typeof(TestEnum));
            _parsersByName.Add("TestEnum", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("TestEnum", "TestEnum")
            };
            var parsedSheet = new ParsedSheet("Item", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_ListAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Container
{{
    public System.Collections.Generic.List<System.Int32> Items {{ get; set; }}
}}
";

            IC4GTypeParser parser = CreateParserForType(typeof(List<int>));
            _parsersByName.Add("IntList", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Items", "IntList")
            };
            var parsedSheet = new ParsedSheet("Container", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_ArrayAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Data
{{
    public System.Int32[] Values {{ get; set; }}
}}
";
            IC4GTypeParser parser = CreateParserForType(typeof(int[]));
            _parsersByName.Add("IntArray", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Values", "IntArray")
            };
            var parsedSheet = new ParsedSheet("Data", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_TwoDimensionalArrayAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Grid
{{
    public System.Int32[,] Matrix {{ get; set; }}
}}
";

            IC4GTypeParser parser = CreateParserForType(typeof(int[,]));
            _parsersByName.Add("IntMatrix", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Matrix", "IntMatrix")
            };
            var parsedSheet = new ParsedSheet("Grid", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_DictionaryAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Mapping
{{
    public System.Collections.Generic.Dictionary<System.Int32, System.String> IntToStringMap {{ get; set; }}
}}
";

            IC4GTypeParser parser = CreateParserForType(typeof(Dictionary<int, string>));
            _parsersByName.Add("IntToStringMap", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("IntToStringMap", "IntToStringMap")
            };
            var parsedSheet = new ParsedSheet("Mapping", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_NestedGenericAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class ComplexData
{{
    public System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.List<System.Int32>> ComplexMap {{ get; set; }}
}}
";
            IC4GTypeParser parser = CreateParserForType(typeof(Dictionary<string, List<int>>));
            _parsersByName.Add("ComplexMap", parser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("ComplexMap", "ComplexMap")
            };
            var parsedSheet = new ParsedSheet("ComplexData", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_MixedAliasesAndRegularTypes()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class MixedClass
{{
    public System.Int32 Health {{ get; set; }}
    public int Level {{ get; set; }}
    public System.Int32[] Scores {{ get; set; }}
}}
";

            IC4GTypeParser healthParser = CreateParserForType(typeof(int));
            IC4GTypeParser intArrayParser = CreateParserForType(typeof(int[]));
            _parsersByName.Add("Health", healthParser);
            _parsersByName.Add("IntArray", intArrayParser);

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Health", "Health"),
                new ParsedPropertyInfo("Level", "int"),
                new ParsedPropertyInfo("Scores", "IntArray")
            };
            var parsedSheet = new ParsedSheet("MixedClass", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_NonExistentAlias()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class TestClass
{{
    public UnknownType Value {{ get; set; }}
}}
";

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Value", "UnknownType")
            };
            var parsedSheet = new ParsedSheet("TestClass", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        [Test]
        public void GenerateDTOClass_EmptyAliasProvider()
        {
            // Arrange
            string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class TestClass
{{
    public Health Hp {{ get; set; }}
    public IntList Items {{ get; set; }}
}}
";

            var propertyInfos = new List<ParsedPropertyInfo>
            {
                new ParsedPropertyInfo("Hp", "Health"),
                new ParsedPropertyInfo("Items", "IntList")
            };
            var parsedSheet = new ParsedSheet("TestClass", propertyInfos, new List<List<string>>());

            // Act
            Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedSheet, _parsersByName);

            // Assert
            Assert.IsTrue(output.IsOk);
            Assert.AreEqual(expectedOutput, output.Value);
        }

        private static IC4GTypeParser CreateParserForType(Type parsingType)
        {
            var parser = Substitute.For<IC4GTypeParser>();
            parser.ParsingType.Returns(parsingType);
            return parser;
        }
    }

    public enum TestEnum
    {
        // ReSharper disable UnusedMember.Global
        Common,
        Rare,
        Epic,
        Legendary
        // ReSharper restore UnusedMember.Global
    }
}