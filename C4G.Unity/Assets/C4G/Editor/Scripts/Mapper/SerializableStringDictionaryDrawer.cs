using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    [CustomPropertyDrawer(typeof(SerializableStringDictionary))]
    public class SerializableStringDictionaryDrawer : PropertyDrawer
    {
        private const float BUTTON_WIDTH = 20f;
        private const float SPACING = 2f;
        private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;

        private Dictionary<string, bool> _foldoutStates = new Dictionary<string, bool>();
        private Dictionary<string, string> _newKeyInputs = new Dictionary<string, string>();
        private Dictionary<string, string> _newValueInputs = new Dictionary<string, string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keysProperty = property.FindPropertyRelative("_keys");
            var valuesProperty = property.FindPropertyRelative("_values");

            if (keysProperty == null || valuesProperty == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid SerializableStringDictionary");
                EditorGUI.EndProperty();
                return;
            }

            string propertyPath = property.propertyPath;
            if (!_foldoutStates.ContainsKey(propertyPath))
                _foldoutStates[propertyPath] = true;

            var currentPos = position;
            currentPos.height = LINE_HEIGHT;

            // Main foldout header
            _foldoutStates[propertyPath] = EditorGUI.Foldout(currentPos, _foldoutStates[propertyPath],
                $"{label.text} ({keysProperty.arraySize} entries)", true);

            if (!_foldoutStates[propertyPath])
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel++;
            currentPos.y += LINE_HEIGHT + SPACING;

            // Add new entry section
            DrawAddNewEntrySection(ref currentPos, propertyPath, keysProperty, valuesProperty);

            currentPos.y += SPACING;

            // Draw existing entries
            DrawExistingEntries(ref currentPos, keysProperty, valuesProperty);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private void DrawAddNewEntrySection(ref Rect currentPos, string propertyPath,
            SerializedProperty keysProperty, SerializedProperty valuesProperty)
        {
            // Initialize input fields if needed
            if (!_newKeyInputs.ContainsKey(propertyPath))
                _newKeyInputs[propertyPath] = "";
            if (!_newValueInputs.ContainsKey(propertyPath))
                _newValueInputs[propertyPath] = "";

            var rect = currentPos;
            rect.height = LINE_HEIGHT;

            // "Add New Entry" label
            EditorGUI.LabelField(rect, "Add New Entry", EditorStyles.boldLabel);
            rect.y += LINE_HEIGHT + SPACING;

            // Key input field
            var keyRect = rect;
            keyRect.width = (rect.width - BUTTON_WIDTH - SPACING) * 0.4f;
            var keyLabelRect = new Rect(keyRect.x, keyRect.y, 30, keyRect.height);
            EditorGUI.LabelField(keyLabelRect, "Key:");
            keyRect.x += 35;
            keyRect.width -= 35;
            _newKeyInputs[propertyPath] = EditorGUI.TextField(keyRect, _newKeyInputs[propertyPath]);

            // Value input field  
            var valueRect = keyRect;
            valueRect.x = keyRect.xMax + SPACING;
            valueRect.width = (rect.width - keyRect.width - BUTTON_WIDTH - SPACING * 2 - 35) * 0.6f;
            EditorGUI.LabelField(new Rect(valueRect.x, valueRect.y, 40, valueRect.height), "Value:");
            valueRect.x += 45;
            valueRect.width -= 45;
            _newValueInputs[propertyPath] = EditorGUI.TextField(valueRect, _newValueInputs[propertyPath]);

            // Add button
            var addButtonRect = new Rect(valueRect.xMax + SPACING, rect.y, BUTTON_WIDTH * 2, LINE_HEIGHT);
            if (GUI.Button(addButtonRect, "+", EditorStyles.miniButton))
            {
                AddNewEntry(propertyPath, keysProperty, valuesProperty);
            }

            currentPos.y = rect.y + LINE_HEIGHT;
        }

        private void DrawExistingEntries(ref Rect currentPos, SerializedProperty keysProperty,
            SerializedProperty valuesProperty)
        {
            if (keysProperty.arraySize == 0)
            {
                var rect = currentPos;
                rect.height = LINE_HEIGHT;
                EditorGUI.LabelField(rect, "No entries", EditorStyles.centeredGreyMiniLabel);
                currentPos.y += LINE_HEIGHT;
                return;
            }

            // Header
            var headerRect = currentPos;
            headerRect.height = LINE_HEIGHT;
            var keyHeaderRect = new Rect(headerRect.x, headerRect.y, headerRect.width * 0.4f, headerRect.height);
            var valueHeaderRect = new Rect(keyHeaderRect.xMax + SPACING, headerRect.y,
                headerRect.width * 0.6f - BUTTON_WIDTH - SPACING, headerRect.height);

            GUI.Box(keyHeaderRect, "Key", EditorStyles.toolbar);
            GUI.Box(valueHeaderRect, "Value", EditorStyles.toolbar);

            currentPos.y += LINE_HEIGHT;

            // Draw entries
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                DrawEntry(ref currentPos, i, keysProperty, valuesProperty);
            }
        }

        private void DrawEntry(ref Rect currentPos, int index, SerializedProperty keysProperty,
            SerializedProperty valuesProperty)
        {
            var rect = currentPos;
            rect.height = LINE_HEIGHT;

            var keyProperty = keysProperty.GetArrayElementAtIndex(index);
            var valueProperty = valuesProperty.GetArrayElementAtIndex(index);

            // Key field
            var keyRect = new Rect(rect.x, rect.y, rect.width * 0.4f - SPACING * 0.5f, rect.height);
            EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);

            // Value field
            var valueRect = new Rect(keyRect.xMax + SPACING, rect.y,
                rect.width * 0.6f - BUTTON_WIDTH - SPACING * 1.5f, rect.height);
            EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

            // Delete button
            var deleteRect = new Rect(valueRect.xMax + SPACING, rect.y, BUTTON_WIDTH, rect.height);
            if (GUI.Button(deleteRect, "×", EditorStyles.miniButtonRight))
            {
                DeleteEntry(index, keysProperty, valuesProperty);
                GUIUtility.ExitGUI();
            }

            currentPos.y += LINE_HEIGHT + SPACING;
        }

        private void AddNewEntry(string propertyPath, SerializedProperty keysProperty,
            SerializedProperty valuesProperty)
        {
            string newKey = _newKeyInputs[propertyPath];
            string newValue = _newValueInputs[propertyPath];

            if (string.IsNullOrEmpty(newKey))
            {
                EditorUtility.DisplayDialog("Invalid Key", "Key cannot be empty!", "OK");
                return;
            }

            // Check for duplicate keys
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                if (keysProperty.GetArrayElementAtIndex(i).stringValue == newKey)
                {
                    EditorUtility.DisplayDialog("Duplicate Key", $"Key '{newKey}' already exists!", "OK");
                    return;
                }
            }

            // Add new entry
            keysProperty.arraySize++;
            valuesProperty.arraySize++;

            var newKeyProperty = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
            var newValueProperty = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);

            newKeyProperty.stringValue = newKey;
            newValueProperty.stringValue = newValue ?? "";

            // Clear input fields
            _newKeyInputs[propertyPath] = "";
            _newValueInputs[propertyPath] = "";

            // Mark as dirty
            EditorUtility.SetDirty(keysProperty.serializedObject.targetObject);
        }

        private void DeleteEntry(int index, SerializedProperty keysProperty, SerializedProperty valuesProperty)
        {
            if (index >= 0 && index < keysProperty.arraySize)
            {
                keysProperty.DeleteArrayElementAtIndex(index);
                valuesProperty.DeleteArrayElementAtIndex(index);
                EditorUtility.SetDirty(keysProperty.serializedObject.targetObject);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string propertyPath = property.propertyPath;
            if (!_foldoutStates.ContainsKey(propertyPath) || !_foldoutStates[propertyPath])
                return LINE_HEIGHT;

            var keysProperty = property.FindPropertyRelative("_keys");
            if (keysProperty == null)
                return LINE_HEIGHT;

            float height = LINE_HEIGHT; // Main foldout
            height += LINE_HEIGHT + SPACING; // "Add New Entry" label
            height += LINE_HEIGHT + SPACING; // Input fields
            height += LINE_HEIGHT + SPACING; // Header

            // Entries (including empty state)
            int entryCount = Mathf.Max(1, keysProperty.arraySize);
            height += entryCount * (LINE_HEIGHT + SPACING);

            return height;
        }
    }
}