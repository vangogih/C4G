using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
	public partial class ConfigsSerializerTests
	{
		public sealed class EdgeCaseTests : ConfigsSerializerTests
		{
			[Test]
			public void SerializeParsedConfigs_NullInput_ReturnsError()
			{
				Result<string, string> result = _configsSerializer.SerializeParsedConfigsAsJsonObject(null, _parsersByName);

				Assert.IsFalse(result.IsOk);
			}

			[Test]
			public void SerializeParsedConfigs_DuplicateSheetName_ReturnsError()
			{
				var config1 = new ParsedConfig("Dup", new List<ParsedPropertyInfo>(), new List<List<string>>());
				var config2 = new ParsedConfig("Dup", new List<ParsedPropertyInfo>(), new List<List<string>>());

				Result<string, string> result = _configsSerializer.SerializeParsedConfigsAsJsonObject(
					new List<ParsedConfig> { config1, config2 }, _parsersByName);

				Assert.IsFalse(result.IsOk);
				Assert.That(result.Error, Does.Contain("Duplicate"));
			}

			[Test]
			public void SerializeParsedConfigs_DuplicatePropertyNames_ReturnsError()
			{
				var properties = new List<ParsedPropertyInfo>
				{
					new ParsedPropertyInfo("Id", "int"),
					new ParsedPropertyInfo("Id", "int")
				};
				var entities = new List<List<string>>
				{
					new List<string> { "1", "2" }
				};
				var config = new ParsedConfig("Sheet", properties, entities);

				Result<string, string> result = _configsSerializer.SerializeParsedConfigsAsJsonObject(
					new List<ParsedConfig> { config }, _parsersByName);

				Assert.IsFalse(result.IsOk);
				Assert.That(result.Error, Does.Contain("duplicated"));
			}

			[Test]
			public void ParseToEntitiesList_NullName_ReturnsError()
			{
				var config = new ParsedConfig(null, new List<ParsedPropertyInfo>(), new List<List<string>>());

				var result = _configsSerializer.ParseToEntitiesList(config, _parsersByName);

				Assert.IsFalse(result.IsOk);
			}

			[Test]
			public void ParseToEntitiesList_NullProperties_ReturnsError()
			{
				var config = new ParsedConfig("Name", null, new List<List<string>>());

				var result = _configsSerializer.ParseToEntitiesList(config, _parsersByName);

				Assert.IsFalse(result.IsOk);
			}

			[Test]
			public void ParseToEntitiesList_NullEntities_ReturnsError()
			{
				var config = new ParsedConfig("Name", new List<ParsedPropertyInfo>(), null);

				var result = _configsSerializer.ParseToEntitiesList(config, _parsersByName);

				Assert.IsFalse(result.IsOk);
			}
		}
	}
}
