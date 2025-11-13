using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.SheetsParsing;

namespace C4G.Core.Settings
{
    public readonly struct C4GSettings
    {
        public readonly string TableId;
        public readonly string RootConfigName;
        public readonly string ClientSecret;
        public readonly string GeneratedCodeFolderFullPath;
        public readonly string SerializedConfigsFolderFullPath;
        public readonly IReadOnlyDictionary<string, SheetParserBase> SheetParsersByName;
        public readonly IReadOnlyDictionary<string, IC4GTypeParser> AliasParsersByName;

        public C4GSettings(
            string tableId,
            string rootConfigName,
            string clientSecret,
            string generatedCodeFolderFullPath,
            string serializedConfigsFolderFullPath,
            IReadOnlyDictionary<string, SheetParserBase> sheetParsersByName,
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