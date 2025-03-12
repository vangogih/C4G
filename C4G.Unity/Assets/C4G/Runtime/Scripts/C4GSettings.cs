using System.IO;
using UnityEngine;

namespace C4G.Runtime.Scripts
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
        public string GeneratedCodeFolderFullPath => Path.GetFullPath(Path.Combine(Application.dataPath, _generatedCodeFolderPath));
        public string GeneratedDataFolderFullPath => Path.GetFullPath(Path.Combine(Application.dataPath, _generatedDataFolderPath));
    }
}