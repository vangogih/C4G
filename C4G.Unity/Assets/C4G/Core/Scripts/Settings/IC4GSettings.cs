using System.Collections.Generic;

namespace C4G.Core.Settings
{
    public interface IC4GSettings
    {
        string TableId { get; }
        string RootConfigName { get; }
        string ClientSecret { get; }
        string GeneratedCodeFolderFullPath { get; }
        string SerializedConfigsFolderFullPath { get; }
        IReadOnlyList<string> SheetNames { get; }
    }
}