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
	[TestFixture]
	public class CodeGeneratorRootConfigTests
	{
		private CodeGenerator _codeGenerator;
		private Dictionary<string, IC4GTypeParser> _parsersByName;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_codeGenerator = new CodeGenerator();
			_parsersByName = new Dictionary<string, IC4GTypeParser>();
		}

		[Test]
		public void GenerateRootConfigClass_SingleConfig()
		{
			string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class GameConfig
{{
    public List<Monster> Monster {{ get; set; }} = new List<Monster>();
}}
";
			var configs = new List<ParsedConfig>
			{
				new ParsedConfig("Monster", new List<ParsedPropertyInfo>(), new List<List<string>>())
			};

			Result<string, string> output = _codeGenerator.GenerateRootConfigClass("GameConfig", configs);

			Assert.IsTrue(output.IsOk);
			Assert.AreEqual(expectedOutput, output.Value);
		}

		[Test]
		public void GenerateRootConfigClass_MultipleConfigs()
		{
			string expectedOutput =
$@"{CodeWriter.GENERATED_CODE_DISCLAIMER}

using System.Collections.Generic;

public partial class Root
{{
    public List<Hero> Hero {{ get; set; }} = new List<Hero>();
    public List<Item> Item {{ get; set; }} = new List<Item>();
}}
";
			var configs = new List<ParsedConfig>
			{
				new ParsedConfig("Hero", new List<ParsedPropertyInfo>(), new List<List<string>>()),
				new ParsedConfig("Item", new List<ParsedPropertyInfo>(), new List<List<string>>())
			};

			Result<string, string> output = _codeGenerator.GenerateRootConfigClass("Root", configs);

			Assert.IsTrue(output.IsOk);
			Assert.AreEqual(expectedOutput, output.Value);
		}

		[Test]
		public void GenerateRootConfigClass_EmptyConfigs()
		{
			var configs = new List<ParsedConfig>();

			Result<string, string> output = _codeGenerator.GenerateRootConfigClass("Empty", configs);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain("public partial class Empty"));
		}

		[Test]
		public void GenerateDTOClass_WithNonGenericNonArrayAlias()
		{
			var parser = Substitute.For<IC4GTypeParser>();
			parser.ParsingType.Returns(typeof(DateTime));
			_parsersByName = new Dictionary<string, IC4GTypeParser> { { "Timestamp", parser } };

			var propertyInfos = new List<ParsedPropertyInfo>
			{
				new ParsedPropertyInfo("CreatedAt", "Timestamp")
			};
			var parsedConfig = new ParsedConfig("Event", propertyInfos, new List<List<string>>());

			Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedConfig, _parsersByName);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain("System.DateTime"));
		}

		[Test]
		public void GenerateDTOClass_WithBaseClass_ViaCodeWriter()
		{
			var propertyInfos = new List<ParsedPropertyInfo>
			{
				new ParsedPropertyInfo("Id", "int")
			};
			var parsedConfig = new ParsedConfig("Child", propertyInfos, new List<List<string>>());

			Result<string, string> output = _codeGenerator.GenerateDTOClass(parsedConfig, _parsersByName);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain("public partial class Child"));
		}
	}
}
