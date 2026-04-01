using System.Collections.Generic;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization
{
	public interface IConfigsSerializer
	{
		Result<string, string> SerializeParsedConfigsAsJsonObject(List<ParsedConfig> parsedConfigs, IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName);
	}
}
