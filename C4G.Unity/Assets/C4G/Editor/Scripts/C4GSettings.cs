using System.Collections.Generic;
using System.IO;
using System.Linq;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using UnityEngine;

namespace C4G.Editor
{
    [CreateAssetMenu(menuName = "C4G/Settings")]
    public class C4GSettings : ScriptableObject, IC4GSettings
    {
        [SerializeField] private string _tableId;
        [SerializeField] private string _rootConfigName;
        [SerializeField] private string _clientSecret;
        [SerializeField, FolderReference] private string _generatedCodeFolderPath;
        [SerializeField, FolderReference] private string _serializedConfigsFolderPath;
        [SerializeField] private List<SheetEntry> _sheets = new List<SheetEntry>();

        public string TableId => _tableId;
        public string RootConfigName => _rootConfigName;
        public string ClientSecret => _clientSecret;

        public string GeneratedCodeFolderFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_generatedCodeFolderPath))
                    return null;
                return Path.GetFullPath(Path.Combine(Application.dataPath, _generatedCodeFolderPath));
            }
        }

        public string SerializedConfigsFolderFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_serializedConfigsFolderPath))
                    return null;
                return Path.GetFullPath(Path.Combine(Application.dataPath, _serializedConfigsFolderPath));
            }
        }

        public bool IsGeneratedCodeFolderValid => !string.IsNullOrEmpty(_generatedCodeFolderPath) &&
                                                  Directory.Exists(GeneratedCodeFolderFullPath);

        public bool IsSerializedConfigsFolderValid => !string.IsNullOrEmpty(_generatedCodeFolderPath) &&
                                                      Directory.Exists(SerializedConfigsFolderFullPath);

        public IReadOnlyDictionary<string, SheetParserBase> SheetConfigurations
        {
            get
            {
                return _sheets
                    .Where(e => e != null && !string.IsNullOrEmpty(e.sheetName) && e.parserBase != null)
                    .ToDictionary(e => e.sheetName, e => e.parserBase);
            }
        }
    }
}