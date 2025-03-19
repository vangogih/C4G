using System.IO;
using UnityEngine;

namespace C4G.Runtime
{
    [CreateAssetMenu(menuName = "C4G/Settings")]
    public class C4GSettings : ScriptableObject
    {
        [SerializeField] private string _tableId;
        [SerializeField] private string _sheetName;
        [SerializeField] private string _clientSecret;
        [SerializeField, FolderReference] private string _generatedCodeFolderPath;
        [SerializeField, FolderReference] private string _generatedDataFolderPath;

        public string TableId => _tableId;
        public string SheetName => _sheetName;
        public string ClientSecret => _clientSecret;

        public bool IsGeneratedCodeFolderValid => !string.IsNullOrEmpty(_generatedCodeFolderPath) &&
                                                  Directory.Exists(GeneratedCodeFolderFullPath);
        public string GeneratedCodeFolderFullPath => Path.GetFullPath(Path.Combine(Application.dataPath, _generatedCodeFolderPath));
        public bool IsGeneratedDataFolderValid => !string.IsNullOrEmpty(_generatedCodeFolderPath) && 
                                                  Directory.Exists(GeneratedDataFolderFullPath);
        public string GeneratedDataFolderFullPath => Path.GetFullPath(Path.Combine(Application.dataPath, _generatedDataFolderPath));
    }
}