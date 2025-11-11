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

        private IReadOnlyDictionary<string, SheetParserBase> _cachedSheetConfigurations;

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
                if (_cachedSheetConfigurations == null)
                {
                    _cachedSheetConfigurations = _sheets
                        .Where(e => e != null && !string.IsNullOrEmpty(e.sheetName) && e.parserBase != null)
                        .GroupBy(e => e.sheetName)
                        .ToDictionary(g => g.Key, g => g.First().parserBase);
                }
                return _cachedSheetConfigurations;
            }
        }

        private void OnValidate()
        {
            _cachedSheetConfigurations = null;
            ValidateUniqueSheetNames();
        }

        private void ValidateUniqueSheetNames()
        {
            var duplicates = _sheets
                .Where(e => e != null && !string.IsNullOrEmpty(e.sheetName))
                .GroupBy(e => e.sheetName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                Debug.LogWarning($"C4GSettings contains duplicate sheet names: {string.Join(", ", duplicates)}. " +
                                 "Only the first occurrence will be used.", this);
            }
        }
    }
}