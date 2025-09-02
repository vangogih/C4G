namespace C4G.Core.Settings
{
    public interface IC4GSettingsFacade
    {
        string TableId { get; }
        string SheetName { get; }
        string ClientSecret { get; }
        string GeneratedCodeFolderFullPath { get; }
        string SerializedConfigsFolderFullPath { get; }
    }
}