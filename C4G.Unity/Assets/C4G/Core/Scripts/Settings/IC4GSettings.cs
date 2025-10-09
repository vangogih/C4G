using System.Collections.Generic;
using C4G.Core.SheetsParsing;

namespace C4G.Core.Settings
{
    public interface IC4GSettings
    {
        string TableId { get; }
        string RootConfigName { get; }
        string ClientSecret { get; }
        string GeneratedCodeFolderFullPath { get; }
        string SerializedConfigsFolderFullPath { get; }
        IReadOnlyList<SheetInfo> SheetInfos { get; }
        SheetParserBase GetParserFor(SheetInfo sheetInfo);
    }
}