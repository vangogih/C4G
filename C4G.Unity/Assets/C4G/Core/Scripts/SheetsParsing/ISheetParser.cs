using System.Collections.Generic;
using C4G.Core.Utils;

namespace C4G.Core.SheetsParsing
{
    public interface ISheetParser
    {
        Result<ParsedSheet, string> Parse(string sheetName, IList<IList<object>> sheetData);
    }
}