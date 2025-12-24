using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing._0_RawParsing
{
    [System.Serializable]
    public abstract class RawSheetParserBase
    {
        public abstract Result<string> ParseNonAlloc(string sheetName, IList<IList<object>> sheetData, List<RawParsedConfig> parsedConfigs);
    }
}