using System.Collections.Generic;
using C4G.Core.Utils;
using C4G.Unity.Assets.C4G.Core.Scripts.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public abstract class SheetParserBase
    {
        public abstract Result<C4GError> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs);
    }
}