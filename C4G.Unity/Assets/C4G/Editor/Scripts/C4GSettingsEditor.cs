using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using C4G.Core.SheetsParsing;

namespace C4G.Editor
{
    [CustomEditor(typeof(C4GSettings))]
    public class C4GSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _tableId;
        private SerializedProperty _rootConfigName;
        private SerializedProperty _clientSecret;
        private SerializedProperty _generatedCodeFolderPath;
        private SerializedProperty _serializedConfigsFolderPath;
        private SerializedProperty _sheets;

        private List<Type> _availableParserTypes;
        private string[] _parserTypeNames;

        private void OnEnable()
        {
            _tableId = serializedObject.FindProperty("_tableId");
            _rootConfigName = serializedObject.FindProperty("_rootConfigName");
            _clientSecret = serializedObject.FindProperty("_clientSecret");
            _generatedCodeFolderPath = serializedObject.FindProperty("_generatedCodeFolderPath");
            _serializedConfigsFolderPath = serializedObject.FindProperty("_serializedConfigsFolderPath");
            _sheets = serializedObject.FindProperty("_sheets");

            // Find all concrete types that inherit from SheetParserBase
            _availableParserTypes = GetAllParserTypes();
            _parserTypeNames = _availableParserTypes.Select(t => t.Name).ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_tableId);
            EditorGUILayout.PropertyField(_rootConfigName);
            EditorGUILayout.PropertyField(_clientSecret);
            EditorGUILayout.PropertyField(_generatedCodeFolderPath);
            EditorGUILayout.PropertyField(_serializedConfigsFolderPath);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Sheets", EditorStyles.boldLabel);

            // Draw sheets list with custom parser selection
            for (int i = 0; i < _sheets.arraySize; i++)
            {
                DrawSheetEntry(i);
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Sheet", GUILayout.Width(100)))
            {
                _sheets.arraySize++;
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSheetEntry(int index)
        {
            SerializedProperty sheetEntry = _sheets.GetArrayElementAtIndex(index);
            SerializedProperty sheetInfo = sheetEntry.FindPropertyRelative("sheetInfo");
            SerializedProperty sheetName = sheetInfo.FindPropertyRelative("sheetName");
            SerializedProperty parserBase = sheetEntry.FindPropertyRelative("parserBase");

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Element {index}", EditorStyles.boldLabel);
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                _sheets.DeleteArrayElementAtIndex(index);
                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(sheetName, new GUIContent("Sheet Name"));

            // Custom dropdown for parser type selection
            DrawParserSelection(parserBase);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawParserSelection(SerializedProperty parserProperty)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Parser Base");

            int currentIndex = -1;
            string currentTypeName = parserProperty.managedReferenceFullTypename;

            if (!string.IsNullOrEmpty(currentTypeName))
            {
                string[] parts = currentTypeName.Split(' ');
                if (parts.Length == 2)
                {
                    string typeFullName = parts[1];
                    Type currentType = _availableParserTypes.FirstOrDefault(t => t.FullName == typeFullName);
                    if (currentType != null)
                    {
                        currentIndex = _availableParserTypes.IndexOf(currentType);
                    }
                }
            }

            // Create dropdown with available parser types
            string[] displayOptions = new string[_parserTypeNames.Length + 1];
            displayOptions[0] = "None";
            Array.Copy(_parserTypeNames, 0, displayOptions, 1, _parserTypeNames.Length);

            int selectedIndex = EditorGUILayout.Popup(currentIndex + 1, displayOptions);

            // Handle selection change
            if (selectedIndex != currentIndex + 1)
            {
                if (selectedIndex == 0)
                {
                    parserProperty.managedReferenceValue = null;
                }
                else
                {
                    Type selectedType = _availableParserTypes[selectedIndex - 1];
                    parserProperty.managedReferenceValue = Activator.CreateInstance(selectedType);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private List<Type> GetAllParserTypes()
        {
            var parserBaseType = typeof(SheetParserBase);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass
                               && !type.IsAbstract
                               && type.IsSubclassOf(parserBaseType)
                               && type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
                .OrderBy(type => type.Name)
                .ToList();

            return types;
        }
    }
}