namespace C4G.Core.Settings
{
    public class SettingsFacade
    {
        public readonly string TableId;
        public readonly string SheetName;
        public readonly string ClientSecret;
        public readonly string GeneratedCodeFolderPath;
        public readonly string GeneratedJsonFolderPath;

        public SettingsFacade(string tableId, string sheetName, string clientSecret, string generatedCodeFolderPath, string generatedJsonFolderPath)
        {
            TableId = tableId;
            SheetName = sheetName;
            ClientSecret = clientSecret;
            GeneratedCodeFolderPath = generatedCodeFolderPath;
            GeneratedJsonFolderPath = generatedJsonFolderPath;
        }
    }
}