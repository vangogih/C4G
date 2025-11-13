using System;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    [Serializable]
    public struct SerializedAliasDefinition
    {
        [SerializeField] private string _name;
        [SerializeField] private SerializedC4GTypeParser _parser;

        public string Name => _name;
        public SerializedC4GTypeParser Parser => _parser;
    }

    [Serializable]
    public struct SerializedC4GTypeParser
    {
        public const string ParserTypeFullNameField = nameof(_parserTypeFullName);

        [SerializeField] private string _parserTypeFullName;

        public string ParserTypeFullName => _parserTypeFullName;
    }

    [CustomPropertyDrawer(type: typeof(SerializedC4GTypeParser), useForChildren: true)]
    public sealed class SerializedC4GTypeParserPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent SmallErrorIcon = EditorGUIUtility.IconContent("console.erroricon.sml");
        private static readonly GUIStyle ErrorStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            normal =
            {
                textColor = new Color(0.8f, 0.2f, 0.2f)
            }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            ParserTypeFullNamePropertyGUIData propertyGUIData = GetPropertyGUIData(property);

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            Rect dropdownRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                position.width - EditorGUIUtility.labelWidth,
                EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();

            int selectedIndex = EditorGUI.Popup(dropdownRect, propertyGUIData.CurrentIndex + 1, C4GTypeParserSerializationHelper.ParserTypeNamesWithLeadingNone) - 1;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Change Parser Type");

                if (selectedIndex == -1)
                {
                    propertyGUIData.ParserTypeFullNameProperty.stringValue = null;
                }
                else
                {
                    Type selectedType = C4GTypeParserSerializationHelper.ParserTypes[selectedIndex];
                    propertyGUIData.ParserTypeFullNameProperty.stringValue = selectedType.AssemblyQualifiedName;
                }

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            if (selectedIndex == -1)
            {
                float errorX = position.x;
                float errorY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                Rect errorIconRect = new Rect(errorX + EditorGUI.indentLevel * 15f, errorY, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                GUI.DrawTexture(errorIconRect, SmallErrorIcon.image);

                errorX += EditorGUIUtility.singleLineHeight;

                Rect errorTextRect = new Rect(errorX, errorY, position.width - EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                string errorText = propertyGUIData.HasTypeFullName ? $"Invalid parser type, re-select '{propertyGUIData.ParserTypeFullName}'" : "No parser type selected, please select";
                GUIContent errorContent = new GUIContent(errorText, errorText);
                EditorGUI.LabelField(errorTextRect, errorContent, ErrorStyle);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent _)
        {
            ParserTypeFullNamePropertyGUIData propertyGUIData = GetPropertyGUIData(property);

            if (propertyGUIData.CurrentIndex == -1)
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }

        private static ParserTypeFullNamePropertyGUIData GetPropertyGUIData(SerializedProperty property)
        {
            SerializedProperty parserTypeFullNameProperty = property.FindPropertyRelative(SerializedC4GTypeParser.ParserTypeFullNameField);
            string parserTypeFullName = parserTypeFullNameProperty.stringValue;
            bool hasTypeFullName = !string.IsNullOrEmpty(parserTypeFullName);
            bool typeExists = hasTypeFullName && C4GTypeParserSerializationHelper.TypeIndexByAssemblyQualifiedNameMap.ContainsKey(parserTypeFullName);
            int currentIndex = typeExists ? C4GTypeParserSerializationHelper.TypeIndexByAssemblyQualifiedNameMap[parserTypeFullName] : -1;
            return new ParserTypeFullNamePropertyGUIData(parserTypeFullNameProperty, parserTypeFullName, hasTypeFullName, currentIndex);
        }

        private readonly struct ParserTypeFullNamePropertyGUIData
        {
            public readonly SerializedProperty ParserTypeFullNameProperty;
            public readonly string ParserTypeFullName;
            public readonly bool HasTypeFullName;
            public readonly int CurrentIndex;

            public ParserTypeFullNamePropertyGUIData(
                SerializedProperty parserTypeFullNameProperty,
                string parserTypeFullName,
                bool hasTypeFullName,
                int currentIndex)
            {
                ParserTypeFullNameProperty = parserTypeFullNameProperty;
                ParserTypeFullName = parserTypeFullName;
                HasTypeFullName = hasTypeFullName;
                CurrentIndex = currentIndex;
            }
        }
    }
}