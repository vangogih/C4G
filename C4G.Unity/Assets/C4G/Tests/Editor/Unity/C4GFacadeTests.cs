using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core;
using C4G.Core.CodeGeneration;
using C4G.Core.ConfigsSerialization;
using C4G.Core.GoogleInteraction;
using C4G.Core.IO;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NSubstitute;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
	[TestFixture]
	public class C4GFacadeTests
	{
		private IC4GSettingsProvider _settingsProvider;
		private IGoogleInteraction _googleInteraction;
		private IIO _io;
		private ICodeGenerator _codeGenerator;
		private IConfigsSerializer _configsSerializer;
		private C4GFacade _facade;
		private string _tempGenDir;
		private string _tempSerDir;

		[SetUp]
		public void SetUp()
		{
			_settingsProvider = Substitute.For<IC4GSettingsProvider>();
			_googleInteraction = Substitute.For<IGoogleInteraction>();
			_io = Substitute.For<IIO>();
			_codeGenerator = new CodeGenerator();
			_configsSerializer = new ConfigsSerializer();
			_facade = new C4GFacade(_settingsProvider, _googleInteraction, _io, _codeGenerator, _configsSerializer);

			_tempGenDir = Path.Combine(Path.GetTempPath(), "C4G_Facade_Gen_" + System.Guid.NewGuid().ToString("N"));
			_tempSerDir = Path.Combine(Path.GetTempPath(), "C4G_Facade_Ser_" + System.Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(_tempGenDir);
			Directory.CreateDirectory(_tempSerDir);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(_tempGenDir))
				Directory.Delete(_tempGenDir, recursive: true);
			if (Directory.Exists(_tempSerDir))
				Directory.Delete(_tempSerDir, recursive: true);
		}

		private C4GSettings CreateValidSettings(
			Dictionary<string, SheetParserBase> sheetParsers = null,
			Dictionary<string, IC4GTypeParser> aliasParsers = null)
		{
			return new C4GSettings(
				tableId: "table123",
				rootConfigName: "Root",
				clientSecret: "secret",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: sheetParsers ?? new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: aliasParsers ?? new Dictionary<string, IC4GTypeParser>());
		}

		[Test]
		public async Task RunAsync_CancelledToken_ReturnsError()
		{
			var cts = new CancellationTokenSource();
			cts.Cancel();

			Result<string> result = await _facade.RunAsync(cts.Token);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("cancelled"));
		}

		[Test]
		public async Task RunAsync_SettingsProviderFails_ReturnsError()
		{
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromError("settings broken"));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public async Task RunAsync_NullTableId_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: null,
				rootConfigName: "Root",
				clientSecret: "secret",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Table id"));
		}

		[Test]
		public async Task RunAsync_NullClientSecret_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "Root",
				clientSecret: null,
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Client secret"));
		}

		[Test]
		public async Task RunAsync_NullRootConfigName_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: null,
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Root config name"));
		}

		[Test]
		public async Task RunAsync_NullGeneratedCodeFolder_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: null,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Generated code folder full path"));
		}

		[Test]
		public async Task RunAsync_NonExistentGeneratedCodeFolder_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: "/non/existent/path/abc123",
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Generated code folder"));
		}

		[Test]
		public async Task RunAsync_NullSerializedConfigsFolder_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: null,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Serialized configs folder full path"));
		}

		[Test]
		public async Task RunAsync_NonExistentSerializedConfigsFolder_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: "/non/existent/ser/abc123",
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Serialized configs folder"));
		}

		[Test]
		public async Task RunAsync_NullSheetParsers_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: null,
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Sheet parsers"));
		}

		[Test]
		public async Task RunAsync_EmptySheetParserKey_ReturnsValidationError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "", new VerticalSheetParser() }
			};
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: sheetParsers,
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Sheet name is null or empty"));
		}

		[Test]
		public async Task RunAsync_NullSheetParserValue_ReturnsValidationError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", null }
			};
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: sheetParsers,
				aliasParsersByName: new Dictionary<string, IC4GTypeParser>());
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Sheet parser for sheet name"));
		}

		[Test]
		public async Task RunAsync_NullAliasParsersByName_ReturnsValidationError()
		{
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: null);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Alias parser by name"));
		}

		[Test]
		public async Task RunAsync_NullAliasParserValue_ReturnsValidationError()
		{
			var aliasParsers = new Dictionary<string, IC4GTypeParser>
			{
				{ "MyAlias", null }
			};
			var settings = new C4GSettings(
				tableId: "t",
				rootConfigName: "R",
				clientSecret: "s",
				generatedCodeFolderFullPath: _tempGenDir,
				serializedConfigsFolderFullPath: _tempSerDir,
				sheetParsersByName: new Dictionary<string, SheetParserBase>(),
				aliasParsersByName: aliasParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("Alias parser with name"));
		}

		[Test]
		public async Task RunAsync_GoogleLoadFails_ReturnsError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromError("google fail")));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public async Task RunAsync_CancelledDuringGoogleLoad_ReturnsError()
		{
			var cts = new CancellationTokenSource();
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(callInfo =>
				{
					cts.Cancel();
					return Task.FromResult(Result<IList<IList<object>>, string>.FromValue(
						(IList<IList<object>>)new List<IList<object>>()));
				});

			Result<string> result = await _facade.RunAsync(cts.Token);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("cancelled"));
		}

		[Test]
		public async Task RunAsync_SheetParsingFails_ReturnsError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			var emptySheet = (IList<IList<object>>)new List<IList<object>>();
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(emptySheet)));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public async Task RunAsync_DtoWriteFails_ReturnsError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			_io.WriteToFile(Arg.Any<string>(), Arg.Is<string>(f => f.EndsWith(".cs")), Arg.Any<string>())
				.Returns(Result<string>.FromError("write dto failed"));

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public async Task RunAsync_RootConfigWriteFails_ReturnsError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			int writeCallCount = 0;
			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(callInfo =>
				{
					writeCallCount++;
					if (writeCallCount == 1)
						return Result<string>.Ok;
					if (writeCallCount == 2)
						return Result<string>.FromError("root config write failed");
					return Result<string>.Ok;
				});

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public async Task RunAsync_SerializedConfigWriteFails_ReturnsError()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			int writeCallCount = 0;
			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(callInfo =>
				{
					writeCallCount++;
					if (writeCallCount <= 2)
						return Result<string>.Ok;
					return Result<string>.FromError("serialized config write failed");
				});

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
		}

		[Test]
		public async Task RunAsync_FullSuccessPath_ReturnsOk()
		{
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Result<string>.Ok);

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsTrue(result.IsOk);
		}

		[Test]
		public async Task RunAsync_NoSheets_SucceedsWithEmptyConfig()
		{
			var settings = CreateValidSettings();
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Result<string>.Ok);

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsTrue(result.IsOk);
		}

		[Test]
		public async Task RunAsync_FullSuccessWithValidAliases_ReturnsOk()
		{
			var aliasParser = Substitute.For<IC4GTypeParser>();
			var aliasParsers = new Dictionary<string, IC4GTypeParser>
			{
				{ "Health", aliasParser }
			};
			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers, aliasParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Result<string>.Ok);

			Result<string> result = await _facade.RunAsync(CancellationToken.None);

			Assert.IsTrue(result.IsOk);
		}

		[Test]
		public async Task RunAsync_DTOClassGenerationFails_ReturnsError()
		{
			var mockCodeGen = Substitute.For<ICodeGenerator>();
			mockCodeGen.GenerateDTOClass(Arg.Any<ParsedConfig>(), Arg.Any<IReadOnlyDictionary<string, IC4GTypeParser>>())
				.Returns(Result<string, string>.FromError("dto gen failed"));
			var facade = new C4GFacade(_settingsProvider, _googleInteraction, _io, mockCodeGen, _configsSerializer);

			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			Result<string> result = await facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("dto gen failed"));
		}

		[Test]
		public async Task RunAsync_RootConfigGenerationFails_ReturnsError()
		{
			var mockCodeGen = Substitute.For<ICodeGenerator>();
			mockCodeGen.GenerateDTOClass(Arg.Any<ParsedConfig>(), Arg.Any<IReadOnlyDictionary<string, IC4GTypeParser>>())
				.Returns(Result<string, string>.FromValue("class Sheet1 {}"));
			mockCodeGen.GenerateRootConfigClass(Arg.Any<string>(), Arg.Any<List<ParsedConfig>>())
				.Returns(Result<string, string>.FromError("root gen failed"));
			var facade = new C4GFacade(_settingsProvider, _googleInteraction, _io, mockCodeGen, _configsSerializer);

			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Result<string>.Ok);

			Result<string> result = await facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("root gen failed"));
		}

		[Test]
		public async Task RunAsync_SerializationFails_ReturnsError()
		{
			var mockSerializer = Substitute.For<IConfigsSerializer>();
			mockSerializer.SerializeParsedConfigsAsJsonObject(Arg.Any<List<ParsedConfig>>(), Arg.Any<IReadOnlyDictionary<string, IC4GTypeParser>>())
				.Returns(Result<string, string>.FromError("serialization failed"));
			var facade = new C4GFacade(_settingsProvider, _googleInteraction, _io, _codeGenerator, mockSerializer);

			var sheetParsers = new Dictionary<string, SheetParserBase>
			{
				{ "Sheet1", new VerticalSheetParser() }
			};
			var settings = CreateValidSettings(sheetParsers);
			_settingsProvider.GetSettings().Returns(Result<C4GSettings, string>.FromValue(settings));

			IList<IList<object>> sheetData = new List<IList<object>>
			{
				new List<object> { "Id" },
				new List<object> { "int" },
				new List<object> { "1" }
			};
			_googleInteraction.LoadSheetAsync("Sheet1", "table123", "secret", Arg.Any<CancellationToken>())
				.Returns(Task.FromResult(Result<IList<IList<object>>, string>.FromValue(sheetData)));

			_io.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
				.Returns(Result<string>.Ok);

			Result<string> result = await facade.RunAsync(CancellationToken.None);

			Assert.IsFalse(result.IsOk);
			Assert.That(result.Error, Does.Contain("serialization failed"));
		}
	}
}
