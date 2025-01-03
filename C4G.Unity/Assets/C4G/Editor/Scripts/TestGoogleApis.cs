﻿using System;
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
        private string _inputSecretFilePath = string.Empty;

        [MenuItem("Tools/C4G/Test")]
        public static void ShowWindow()
        {
            GetWindow<TestGoogleApis>("Test Google APIs");
        }

        private void OnGUI()
        {
            GUILayout.Label("Enter a String", EditorStyles.boldLabel);
            _inputSecretFilePath = GetFirstFoundSecretFilePath();
            _inputSecretFilePath = EditorGUILayout.TextField("Secret file path", _inputSecretFilePath);

            if (GUILayout.Button("Submit"))
            {
                Debug.Log("Submitted value: " + _inputSecretFilePath);
                Test();
                Debug.Log("Test finished");
            }
        }

        private static string GetFirstFoundSecretFilePath()
        {
            string[] secretFiles = Directory.GetFiles(Constants.SecretsDirectoryPath, "*.json");
            return secretFiles.Length > 0 ? secretFiles[0] : string.Empty;
        }

        private void Test()
        {
            try
            {
                GoogleCredential googleCredential = GoogleCredential.FromFile(_inputSecretFilePath)
                    .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

                var service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = googleCredential,
                    ApplicationName = "C4G"
                });

                string sheetName = "Sheet1";
                var request = service.Spreadsheets.Values.Get(TableId, sheetName);
                IList<IList<object>> rows = request.Execute().Values;

                ParsedSheet parsedSheet = SheetParser.ParseSheet(sheetName, rows);

                CreateJsonFile(parsedSheet);

                GenerateCode(parsedSheet);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during Test execution: " + ex.Message);
            }
        }

        private static void CreateJsonFile(ParsedSheet parsedSheet)
        {
            string json = ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

            string streamingAssetsPath = Path.GetFullPath(Path.Combine(Application.dataPath, "StreamingAssets"));

            if (!Directory.Exists(streamingAssetsPath))
            {
                Directory.CreateDirectory(streamingAssetsPath);
            }

            string filePath = Path.Combine(streamingAssetsPath, $"{parsedSheet.Name}.json");

            File.WriteAllText(filePath, json);
        }

        private static void GenerateCode(ParsedSheet parsedSheet)
        {
            string dtoClass = CodeGenerator.GenerateDTOClass(parsedSheet);
            string wrapperClass = CodeGenerator.GenerateWrapperClass(parsedSheet);
            string destinationFolderPath = Path.GetFullPath(Path.Combine(Application.dataPath,
                "C4G", "Runtime", "Scripts", "Generated"));
            if (!Directory.Exists(destinationFolderPath))
                Directory.CreateDirectory(destinationFolderPath);
            string dtoClassFilePath = Path.Combine(destinationFolderPath, $"{parsedSheet.Name}.cs");
            File.WriteAllText(dtoClassFilePath, dtoClass);
            string wrapperClassFilePath = Path.Combine(destinationFolderPath, $"{parsedSheet.Name}Wrapper.cs");
            File.WriteAllText(wrapperClassFilePath, wrapperClass);
        }
    }
}