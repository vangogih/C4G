using System;
using System.Collections.Generic;
using System.Linq;
using C4G.Core.SheetsParsing;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    [CustomPropertyDrawer(typeof(SheetEntry))]
    public class SheetEntryDrawer : PropertyDrawer
    {
        private static List<Type> _availableParserTypes;
        private static string[] _parserTypeNames;
        private static string[] _displayOptions;

        private const float Spacing = 2f;
        private const float LineHeight = 18f;

        static SheetEntryDrawer()
        {
            _availableParserTypes = GetAllParserTypes();
            _parserTypeNames = _availableParserTypes.Select(t => t.Name).ToArray();

            _displayOptions = new string[_parserTypeNames.Length + 1];
            _displayOptions[0] = "None";
            Array.Copy(_parserTypeNames, 0, _displayOptions, 1, _parserTypeNames.Length);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LineHeight * 2 + Spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty sheetName = property.FindPropertyRelative(nameof(SheetEntry.sheetName));
            SerializedProperty parserBase = property.FindPropertyRelative(nameof(SheetEntry.parserBase));

            Rect sheetNameRect = new Rect(position.x, position.y, position.width, LineHeight);
            EditorGUI.PropertyField(sheetNameRect, sheetName, new GUIContent("Sheet Name"));

            Rect parserRect = new Rect(position.x, position.y + LineHeight + Spacing, position.width, LineHeight);
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

                    for (int i = 0; i < _availableParserTypes.Count; i++)
                    {
                        if (string.Equals(_availableParserTypes[i].FullName, typeFullName, StringComparison.Ordinal))
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

            int selectedIndex = EditorGUI.Popup(popupRect, currentIndex + 1, _displayOptions);

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
        }

        private static List<Type> GetAllParserTypes()
        {
            var parserBaseType = typeof(SheetParserBase);

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