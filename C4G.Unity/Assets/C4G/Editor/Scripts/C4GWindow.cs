using System;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core;
using C4G.Core.Utils;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
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

            if (!_settings.IsSerializedConfigsFolderValid)
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
                C4GFacade c4gFacade = new C4GFacade(_settings);

                Task<Result<string>> c4gRunTask = c4gFacade.RunAsync(cts.Token);
                while (!c4gRunTask.IsCompleted && !c4gRunTask.IsCanceled && !c4gRunTask.IsFaulted)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("C4G", "Loading...", 0.5f))
                        cts.Cancel();

                    await Task.Delay(50, cts.Token);
                }

                Result<string> c4gRunResult = await c4gRunTask;

                if (c4gRunResult.IsOk)
                    Debug.Log($"{LOG_TAG} Successful Run");
                else
                    Debug.LogError($"{LOG_TAG} Error During Run {c4gRunResult.Error}");
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
                _isConfigsUpdateInProgress = false;
                EditorUtility.ClearProgressBar();
            }
        }
    }
}