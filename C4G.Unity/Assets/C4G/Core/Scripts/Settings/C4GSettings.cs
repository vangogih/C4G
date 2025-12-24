using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;
using C4G.Core.SheetsParsing._0_RawParsing;

namespace C4G.Core.Settings
{
    public readonly struct C4GSettings
    {
        public readonly string TableId;
        public readonly string RootConfigName;
        public readonly string ClientSecret;
        public readonly string GeneratedCodeFolderFullPath;
        public readonly string SerializedConfigsFolderFullPath;
        public readonly IReadOnlyDictionary<string, RawSheetParserBase> SheetParsersByName;
        public readonly IReadOnlyDictionary<string, IC4GTypeParser> AliasParsersByName;

        public C4GSettings(
            string tableId,
            string rootConfigName,
            string clientSecret,
            string generatedCodeFolderFullPath,
            string serializedConfigsFolderFullPath,
            IReadOnlyDictionary<string, RawSheetParserBase> sheetParsersByName,
            IReadOnlyDictionary<string, IC4GTypeParser> aliasParsersByName)
        {
            TableId = tableId;
            RootConfigName = rootConfigName;
            ClientSecret = clientSecret;
            GeneratedCodeFolderFullPath = generatedCodeFolderFullPath;
            SerializedConfigsFolderFullPath = serializedConfigsFolderFullPath;
            SheetParsersByName = sheetParsersByName;
            AliasParsersByName = aliasParsersByName;
        }
    }
}