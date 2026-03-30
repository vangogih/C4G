using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core.CodeGeneration
{
	public interface ICodeGenerator
	{
		Result<string, string> GenerateDTOClass(ParsedConfig parsedConfig, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName);
		Result<string, string> GenerateRootConfigClass(string name, List<ParsedConfig> parsedConfigs);
	}
}
