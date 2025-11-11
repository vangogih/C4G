using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    [System.Serializable]
    public abstract class SheetParserBase
    {
        public abstract Result<ParsedSheet, string> Parse(string sheetName, IList<IList<object>> sheetData);
        public abstract string Validate(IList<IList<object>> sheetData);
    }
}