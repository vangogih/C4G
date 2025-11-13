using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Settings;
using C4G.Core.SheetsParsing;
using C4G.Core.Utils;
using UnityEngine;

namespace C4G.Editor
{
    [CreateAssetMenu(menuName = "C4G/Settings")]
    public sealed class C4GSettingsProvider : ScriptableObject, IC4GSettingsProvider
    {
        [SerializeField] private string _tableId;
        [SerializeField] private string _rootConfigName;
        [SerializeField] private string _clientSecret;
        [SerializeField, FolderReference] private string _generatedCodeFolderPath;
        [SerializeField, FolderReference] private string _serializedConfigsFolderPath;
        [SerializeField] private List<SerializedSheetDefinition> _sheetDefinitions = new List<SerializedSheetDefinition>();
        [SerializeField] private List<SerializedAliasDefinition> _aliasDefinitions = new List<SerializedAliasDefinition>();

        Result<C4GSettings, string> IC4GSettingsProvider.GetSettings()
        {
            var sheetParsersByName = new Dictionary<string, SheetParserBase>(_sheetDefinitions.Count, StringComparer.Ordinal);
            foreach (SerializedSheetDefinition sheetDefinition in _sheetDefinitions)
            {
                string sheetName = sheetDefinition.Name;

                if (string.IsNullOrEmpty(sheetName))
                    return Result<C4GSettings, string>.FromError($"C4G Error. Null or empty sheet name");

                if (sheetParsersByName.ContainsKey(sheetName))
                    return Result<C4GSettings, string>.FromError($"C4G Error. Duplicated sheet name '{sheetName}'");

                if (sheetDefinition.Parser == null)
                    return Result<C4GSettings, string>.FromError($"C4G Error. Parser is null for sheet name '{sheetName}'");

                sheetParsersByName.Add(sheetName, sheetDefinition.Parser);
            }

            var aliasParsersByName = new Dictionary<string, IC4GTypeParser>(_aliasDefinitions.Count, StringComparer.Ordinal);
            foreach (SerializedAliasDefinition aliasDefinition in _aliasDefinitions)
            {
                string parserTypeFullName = aliasDefinition.Parser.ParserTypeFullName;
                if (!string.IsNullOrEmpty(parserTypeFullName) && C4GTypeParserSerializationHelper.TypeIndexByAssemblyQualifiedNameMap.TryGetValue(parserTypeFullName, out int parserTypeIndex))
                {
                    string aliasName = aliasDefinition.Name;

                    if (string.IsNullOrEmpty(aliasName))
                        return Result<C4GSettings, string>.FromError($"C4G Error. Null or empty alias name");

                    if (aliasParsersByName.ContainsKey(aliasName))
                        return Result<C4GSettings, string>.FromError($"C4G Error. Duplicated alias name '{aliasName}'");

                    Type parserType = C4GTypeParserSerializationHelper.ParserTypes[parserTypeIndex];
                    IC4GTypeParser parser = (IC4GTypeParser)Activator.CreateInstance(parserType);
                    aliasParsersByName.Add(aliasName, parser);
                }
            }

            var settings = new C4GSettings(
                _tableId,
                _rootConfigName,
                _clientSecret,
                GeneratedCodeFolderFullPath,
                SerializedConfigsFolderFullPath,
                sheetParsersByName,
                aliasParsersByName);

            return Result<C4GSettings, string>.FromValue(settings);
        }

        public string TableId => _tableId;
        public string RootConfigName => _rootConfigName;
        public string ClientSecret => _clientSecret;

        public string GeneratedCodeFolderFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_generatedCodeFolderPath))
                    return null;
                string assetsFolder = Application.dataPath;
                string projectFolder = Directory.GetParent(assetsFolder).FullName;
                return Path.GetFullPath(Path.Combine(projectFolder, _generatedCodeFolderPath));
            }
        }

        public string SerializedConfigsFolderFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_serializedConfigsFolderPath))
                    return null;
                string assetsFolder = Application.dataPath;
                string projectFolder = Directory.GetParent(assetsFolder).FullName;
                return Path.GetFullPath(Path.Combine(projectFolder, _serializedConfigsFolderPath));
            }
        }

        public bool IsGeneratedCodeFolderValid => !string.IsNullOrEmpty(_generatedCodeFolderPath) &&
                                                  Directory.Exists(GeneratedCodeFolderFullPath);

        public bool IsSerializedConfigsFolderValid => !string.IsNullOrEmpty(_generatedCodeFolderPath) &&
                                                      Directory.Exists(SerializedConfigsFolderFullPath);

        private void OnValidate()
        {
            ValidateUniqueSheetNames();
        }

        private void ValidateUniqueSheetNames()
        {
            var duplicates = _sheetDefinitions
                .Where(e => string.IsNullOrEmpty(e.Name))
                .GroupBy(e => e.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicates.Any())
            {
                Debug.LogWarning($"C4GSettings contains duplicate sheet names: {string.Join(", ", duplicates)}. " +
                                 "Only the first occurrence will be used.", this);
            }
        }
    }
}