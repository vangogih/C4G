using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    public class TestGoogleApis : EditorWindow
    {
        private const string TableId = "1xNbXhj706f2ZLUCqROPkAEki_gRe1CPomkPoWua4ogU";
        private string _inputSecretFileName = String.Empty;

        [MenuItem("Tools/C4G/Test")]
        public static void ShowWindow()
        {
            GetWindow<TestGoogleApis>("Test Google APIs");
        }

        private void OnGUI()
        {
            GUILayout.Label("Enter a String", EditorStyles.boldLabel);
            _inputSecretFileName = GetFirstFoundSecret();
            _inputSecretFileName = EditorGUILayout.TextField("Secret file name", _inputSecretFileName);

            if (GUILayout.Button("Submit"))
            {
                Debug.Log("Submitted value: " + _inputSecretFileName);
                Test();
            }
        }

        private string GetFirstFoundSecret()
        {
            string secretsDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "C4G", "Editor", "Secrets"));

            string[] secretFiles = Directory.GetFiles(secretsDirectory, "*.json");

            return secretFiles.Length > 0 ? Path.GetFileName(secretFiles[0]) : string.Empty;
        }

        private void Test()
        {
            try
            {
                GoogleCredential googleCredential = GoogleCredential.FromFile(Path.GetFullPath(Path.Combine(Application.dataPath,
                    "C4G", "Editor", "Secrets", _inputSecretFileName))).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
                
                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = googleCredential,
                    ApplicationName = "C4G"
                });
                
                string sheetName = "Sheet1";
                var request = service.Spreadsheets.Values.Get(TableId, sheetName);
                IList<IList<object>> rows = request.Execute().Values;
                ParsedSheet parsedSheet = SheetParser.ParseSheet(sheetName, rows);
                
                Debug.Log(parsedSheet.Name);
                
                foreach (ParsedPropertyInfo property in parsedSheet.Properties)
                {
                    Debug.Log($"Property {property.Name}: {property.Type}");
                }
                
                foreach (IReadOnlyCollection<string> entity in parsedSheet.Entities)
                {
                    Debug.Log($"Entity {{{string.Join(", ", entity)}}}");
                }
                
                ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet, sheetName);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during Test execution: " + ex.Message);
            }
        }
    }
}