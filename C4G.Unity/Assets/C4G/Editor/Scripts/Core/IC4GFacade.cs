using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace C4G.Editor.Core
{
    public interface IC4GFacade
    {
        Task<IList<IList<object>>> LoadRawConfigAsync(CancellationToken ct);
        ParsedSheet ParseSheet(string sheetName, IList<IList<object>> rawConfig);
        string ConvertParsedSheetToJsonString(ParsedSheet parsedSheet);
        string GenerateDTOClassFromParsedSheet(ParsedSheet parsedSheet);
        string GenerateWrapperClassFromParsedSheet(ParsedSheet parsedSheet);
    }
}