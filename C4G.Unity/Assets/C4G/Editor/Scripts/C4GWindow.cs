using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using C4G.Runtime.Scripts;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    public sealed class C4GWindow : EditorWindow
    {
        private const string LOG_TAG = "[C4G]";
        private const string C4G_CLIENT_SECRET_PREFS_KEY = "C4G_ClientSecret";

        private bool _pendingRequest;
        private C4GSettings _settings;
        private UnityEditor.Editor _settingsEditor;

        private string _clientSecret;
        private string ClientSecret
        {
            get
            {
                if (string.IsNullOrEmpty(_clientSecret) && EditorPrefs.HasKey(C4G_CLIENT_SECRET_PREFS_KEY))
                    _clientSecret = EditorPrefs.GetString(C4G_CLIENT_SECRET_PREFS_KEY);

                return _clientSecret;
            }
            set
            {
                _clientSecret = value;
                EditorPrefs.SetString(C4G_CLIENT_SECRET_PREFS_KEY, value);
            }
        }

        [MenuItem("Tools/C4G/Open C4G Window")]
        public static void ShowWindow()
        {
            GetWindow<C4GWindow>("C4G");
        }

        private void OnGUI()
        {
            if (_pendingRequest)
            {
                EditorGUILayout.HelpBox("Waiting for pending request...", MessageType.Info);
                GUI.enabled = false;
            }

            if (_settings == null)
            {
                string[] configGuids = AssetDatabase.FindAssets($"t:{nameof(C4GSettings)}");

                if (configGuids.Length == 0)
                {
                    EditorGUILayout.HelpBox("Create C4G settings first", MessageType.Warning);
                    return;
                }

                if (configGuids.Length > 1)
                {
                    EditorGUILayout.HelpBox("Multiple C4G settings assets found. Make sure that one and only one C4G settings asset exists", MessageType.Warning);
                    return;
                }

                string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
                _settings = AssetDatabase.LoadAssetAtPath<C4GSettings>(configPath);
                _settingsEditor = UnityEditor.Editor.CreateEditor(_settings);
            }

            _settingsEditor.OnInspectorGUI();

            ClientSecret = EditorGUILayout.TextField("Client Secret", ClientSecret);

            if (string.IsNullOrEmpty(ClientSecret))
            {
                EditorGUILayout.HelpBox("Setup client secret first", MessageType.Warning);
                GUI.enabled = false;
            }

            if (GUILayout.Button("Update configs"))
                UpdateConfigsAsync();

            GUI.enabled = true;
        }

        private async Task UpdateConfigsAsync()
        {
            if (_pendingRequest)
                return;

            _pendingRequest = true;

            EditorUtility.DisplayProgressBar("C4G", "Updating configs...", 0.1f);

            try
            {
                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    (await GoogleClientSecrets.FromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(ClientSecret)))).Secrets,
                    new[] { SheetsService.Scope.SpreadsheetsReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("C4G"));

                EditorUtility.DisplayProgressBar("C4G", "Updating configs...", 0.5f);

                var sheetsService = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "c4g-test-440907"
                });

                SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(_settings.TableId, _settings.SheetName);
                IList<IList<object>> rows = (await request.ExecuteAsync()).Values;

                ParsedSheet parsedSheet = SheetParser.ParseSheet(_settings.SheetName, rows);

                EditorUtility.DisplayProgressBar("C4G", "Updating configs...", 0.8f);

                CreateJsonFile(parsedSheet, _settings.GeneratedDataFolderFullPath);

                EditorUtility.DisplayProgressBar("C4G", "Updating configs...", 0.9f);

                GenerateCode(parsedSheet, _settings.GeneratedCodeFolderFullPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_TAG} Error during configs update\n" +
                               $"{ex.Message}");
            }

            _pendingRequest = false;

            EditorUtility.ClearProgressBar();
            Debug.Log($"{LOG_TAG} Configs updated");
        }

        private static void CreateJsonFile(ParsedSheet parsedSheet, string folderPath)
        {
            string json = ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, $"{parsedSheet.Name}.json");

            File.WriteAllText(filePath, json);
        }

        private static void GenerateCode(ParsedSheet parsedSheet, string folderPath)
        {
            string dtoClass = CodeGenerator.GenerateDTOClass(parsedSheet);
            string wrapperClass = CodeGenerator.GenerateWrapperClass(parsedSheet);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string dtoClassFilePath = Path.Combine(folderPath, $"{parsedSheet.Name}.cs");
            File.WriteAllText(dtoClassFilePath, dtoClass);

            string wrapperClassFilePath = Path.Combine(folderPath, $"{parsedSheet.Name}Wrapper.cs");
            File.WriteAllText(wrapperClassFilePath, wrapperClass);
        }
    }
}