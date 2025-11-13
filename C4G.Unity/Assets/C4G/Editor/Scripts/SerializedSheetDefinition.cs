using System;
using System.Collections.Generic;
using System.Linq;
using C4G.Core.SheetsParsing;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    [Serializable]
    public struct SerializedSheetDefinition
    {
        public const string NamePropertyName = nameof(_name);
        public const string ParserPropertyName = nameof(_parser);

        [SerializeField] private string _name;
        [SerializeReference] private SheetParserBase _parser;

        public string Name => _name;
        public SheetParserBase Parser => _parser;
    }

    [CustomPropertyDrawer(typeof(SerializedSheetDefinition))]
    public class SerializedSheetDefinitionPropertyDrawer : PropertyDrawer
    {
        private static readonly List<Type> AvailableParserTypes;
        private static readonly string[] DisplayOptions;

        static SerializedSheetDefinitionPropertyDrawer()
        {
            AvailableParserTypes = GetAllParserTypes();
            string[] parserTypeNames = AvailableParserTypes.Select(t => t.Name).ToArray();

            DisplayOptions = new string[parserTypeNames.Length + 1];
            DisplayOptions[0] = "None";
            Array.Copy(parserTypeNames, 0, DisplayOptions, 1, parserTypeNames.Length);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty sheetName = property.FindPropertyRelative(SerializedSheetDefinition.NamePropertyName);
            SerializedProperty parserBase = property.FindPropertyRelative(SerializedSheetDefinition.ParserPropertyName);

            var lineHeight = EditorGUIUtility.singleLineHeight;
            Rect sheetNameRect = new Rect(position.x, position.y, position.width, lineHeight);
            EditorGUI.PropertyField(sheetNameRect, sheetName, new GUIContent("Sheet Name"));

            Rect parserRect = new Rect(position.x, position.y + lineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, lineHeight);
            DrawParserSelection(parserRect, parserBase);

            EditorGUI.EndProperty();
        }

        private void DrawParserSelection(Rect position, SerializedProperty parserProperty)
        {
            int currentIndex = -1;
            string currentTypeName = parserProperty.managedReferenceFullTypename;

            if (!string.IsNullOrEmpty(currentTypeName))
            {
                int spaceIndex = currentTypeName.IndexOf(' ');
                if (spaceIndex > 0 && spaceIndex < currentTypeName.Length - 1)
                {
                    string typeFullName = currentTypeName.Substring(spaceIndex + 1);

                    for (int i = 0; i < AvailableParserTypes.Count; i++)
                    {
                        if (string.Equals(AvailableParserTypes[i].FullName, typeFullName, StringComparison.Ordinal))
                        {
                            currentIndex = i;
                            break;
                        }
                    }
                }
            }

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, "Parser");

            Rect popupRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                position.width - EditorGUIUtility.labelWidth, position.height);

            int selectedIndex = EditorGUI.Popup(popupRect, currentIndex + 1, DisplayOptions);

            if (selectedIndex != currentIndex + 1)
            {
                if (selectedIndex == 0)
                {
                    parserProperty.managedReferenceValue = null;
                }
                else
                {
                    Type selectedType = AvailableParserTypes[selectedIndex - 1];
                    parserProperty.managedReferenceValue = Activator.CreateInstance(selectedType);
                }
            }
        }

        private static List<Type> GetAllParserTypes()
        {
            var derivedTypes = TypeCache.GetTypesDerivedFrom<SheetParserBase>();

            var types = new List<Type>();
            foreach (var type in derivedTypes)
            {
                if (!type.IsAbstract
                    && !type.IsGenericType
                    && type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
                {
                    types.Add(type);
                }
            }

            types.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            return types;
        }
    }
}