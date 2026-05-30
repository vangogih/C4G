using System.Collections.Generic;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public abstract class SheetParserBase
    {
        public abstract Result<C4GSheetsParsingError> ParseToList(string sheetName, IList<IList<object>> sheetData, List<ParsedConfig> parsedConfigs);
    }
}