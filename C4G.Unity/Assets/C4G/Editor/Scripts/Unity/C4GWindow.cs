using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using C4G.Editor.Core;
using C4G.Runtime;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor.Unity
{
    public sealed class C4GWindow : EditorWindow
    {
        private const string LOG_TAG = "[C4G]";

        private bool _isConfigsUpdateInProgress;
        private C4GSettings _settings;
        private UnityEditor.Editor _settingsEditor;

        [MenuItem("Tools/C4G/Open C4G Window")]
        public static void ShowWindow()
        {
            GetWindow<C4GWindow>("C4G");
        }

        private void OnGUI()
        {
            if (_isConfigsUpdateInProgress)
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

            DrawSettingValidationGUI();

            if (GUILayout.Button("Update configs"))
                UpdateConfigsAsync();

            GUI.enabled = true;
        }

        private void DrawSettingValidationGUI()
        {
            if (string.IsNullOrEmpty(_settings.TableId))
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("Please, setup client table id", MessageType.Warning);
                GUI.enabled = false;
            }

            if (string.IsNullOrEmpty(_settings.SheetName))
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("Please, setup sheet name", MessageType.Warning);
                GUI.enabled = false;
            }

            if (string.IsNullOrEmpty(_settings.ClientSecret))
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("Please, setup client secret", MessageType.Warning);
                GUI.enabled = false;
            }

            if (!_settings.IsGeneratedCodeFolderValid)
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("Please, browse valid generated code folder", MessageType.Warning);
                GUI.enabled = false;
            }

            if (!_settings.IsGeneratedDataFolderValid)
            {
                GUI.enabled = true;
                EditorGUILayout.HelpBox("Please, browse valid generated data folder", MessageType.Warning);
                GUI.enabled = false;
            }
        }

        private async Task UpdateConfigsAsync()
        {
            if (_isConfigsUpdateInProgress)
                return;

            _isConfigsUpdateInProgress = true;

            CancellationTokenSource cts = new CancellationTokenSource();

            try
            {
                IC4GFacade facade = new C4GUnityFacade(_settings.TableId, _settings.SheetName, _settings.ClientSecret);

                Task<IList<IList<object>>> loadRawConfigTask = facade.LoadRawConfigAsync(cts.Token);
                while (!loadRawConfigTask.IsCompleted && !loadRawConfigTask.IsCanceled && !loadRawConfigTask.IsFaulted)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("C4G", "Loading raw config...", 0.1f))
                        cts.Cancel();

                    await Task.Delay(50, cts.Token);
                }

                EditorUtility.DisplayProgressBar("C4G", "Parsing config...", 0.5f);

                ParsedSheet parsedSheet = facade.ParseSheet(_settings.SheetName, loadRawConfigTask.Result);

                EditorUtility.DisplayProgressBar("C4G", "Generating data...", 0.8f);

                CreateJsonFile(facade, parsedSheet, _settings.GeneratedDataFolderFullPath);

                EditorUtility.DisplayProgressBar("C4G", "Generating code...", 0.9f);

                GenerateCode(facade, parsedSheet, _settings.GeneratedCodeFolderFullPath);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                Debug.LogError($"{LOG_TAG} Error during configs update\n" +
                               $"{ex}\n");
            }
            finally
            {
                cts.Dispose();
            }

            _isConfigsUpdateInProgress = false;

            EditorUtility.ClearProgressBar();
            Debug.Log($"{LOG_TAG} Configs updated");
        }

        private static void CreateJsonFile(IC4GFacade facade, ParsedSheet parsedSheet, string folderPath)
        {
            string json = facade.ConvertParsedSheetToJsonString(parsedSheet);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, $"{parsedSheet.Name}.json");

            File.WriteAllText(filePath, json);
        }

        private static void GenerateCode(IC4GFacade facade, ParsedSheet parsedSheet, string folderPath)
        {
            string dtoClass = facade.GenerateDTOClassFromParsedSheet(parsedSheet);
            string wrapperClass = facade.GenerateWrapperClassFromParsedSheet(parsedSheet);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string dtoClassFilePath = Path.Combine(folderPath, $"{parsedSheet.Name}.cs");
            File.WriteAllText(dtoClassFilePath, dtoClass);

            string wrapperClassFilePath = Path.Combine(folderPath, $"{parsedSheet.Name}Wrapper.cs");
            File.WriteAllText(wrapperClassFilePath, wrapperClass);
        }
    }
}