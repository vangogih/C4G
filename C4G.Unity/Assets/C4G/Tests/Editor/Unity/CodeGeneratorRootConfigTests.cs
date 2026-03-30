using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using C4G.Core.CodeGeneration;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NSubstitute;
using NUnit.Framework;

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
			var configs = new List<ParsedConfig>
			{
				new ParsedConfig("Monster", new List<ParsedPropertyInfo>(), new List<List<string>>())
			};

			Result<string, string> output = _codeGenerator.GenerateRootConfigClass("GameConfig", configs);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain("public partial class GameConfig"));
			Assert.That(output.Value, Does.Contain("public List<Monster> Monster { get; set; }"));
		}

		[Test]
		public void GenerateRootConfigClass_MultipleConfigs()
		{
			var configs = new List<ParsedConfig>
			{
				new ParsedConfig("Hero", new List<ParsedPropertyInfo>(), new List<List<string>>()),
				new ParsedConfig("Item", new List<ParsedPropertyInfo>(), new List<List<string>>())
			};

			Result<string, string> output = _codeGenerator.GenerateRootConfigClass("Root", configs);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain("public partial class Root"));
			Assert.That(output.Value, Does.Contain("public List<Hero> Hero { get; set; }"));
			Assert.That(output.Value, Does.Contain("public List<Item> Item { get; set; }"));
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

		[Test]
		public void CodeWriter_NonPartial_WithBaseClass()
		{
			var writer = new CodeWriter("    ");
			writer.AddUsing("System");
			writer.WritePublicClass("MyClass", isPartial: false, baseClass: "BaseClass", w =>
			{
				w.WritePublicProperty("Id", "int");
			});
			string result = writer.Build();

			Assert.That(result, Does.Contain("public class MyClass : BaseClass"));
			Assert.That(result, Does.Not.Contain("partial"));
		}

		[Test]
		public void GenerateDTOClass_WithNullFullName_NonGenericAlias()
		{
			var genericType = typeof(List<>);
			Type typeParam = genericType.GetGenericArguments()[0];
			Assert.IsNull(typeParam.FullName);

			var parser = Substitute.For<IC4GTypeParser>();
			parser.ParsingType.Returns(typeParam);
			var aliases = new Dictionary<string, IC4GTypeParser> { { "NullFullName", parser } };

			var props = new List<ParsedPropertyInfo>
			{
				new ParsedPropertyInfo("Val", "NullFullName")
			};
			var config = new ParsedConfig("Cfg", props, new List<List<string>>());

			Result<string, string> output = _codeGenerator.GenerateDTOClass(config, aliases);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain(typeParam.Name));
		}

		[Test]
		public void GenerateDTOClass_WithNullFullName_GenericAlias()
		{
			var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(
				new AssemblyName("TestDynAsm2"), AssemblyBuilderAccess.Run);
			var modBuilder = asmBuilder.DefineDynamicModule("TestDynModule2");
			var genericBuilder = modBuilder.DefineType("DynGeneric`1");
			genericBuilder.DefineGenericParameters("T");
			var constructedGeneric = genericBuilder.MakeGenericType(typeof(int));

			var parser = Substitute.For<IC4GTypeParser>();
			parser.ParsingType.Returns(constructedGeneric);
			var aliases = new Dictionary<string, IC4GTypeParser> { { "GenAlias", parser } };

			var props = new List<ParsedPropertyInfo>
			{
				new ParsedPropertyInfo("Val", "GenAlias")
			};
			var config = new ParsedConfig("Cfg2", props, new List<List<string>>());

			Result<string, string> output = _codeGenerator.GenerateDTOClass(config, aliases);

			Assert.IsTrue(output.IsOk);
			Assert.That(output.Value, Does.Contain("DynGeneric"));
			Assert.That(output.Value, Does.Contain("System.Int32"));
		}
	}
}
