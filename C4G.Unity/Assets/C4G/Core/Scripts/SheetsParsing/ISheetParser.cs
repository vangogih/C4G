using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public abstract class SheetParserBase
    {
        public abstract Result<ParsedSheet, string> Parse(string sheetName, IList<IList<object>> sheetData);
    }
}