using C4G.Runtime.Scripts;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    [CustomPropertyDrawer(typeof(FolderReferenceAttribute))]
    internal sealed class FolderReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [FolderReference] with string fields only");
                return;
            }

            var textFieldRect = position;
            textFieldRect.width -= 60;
        
            var buttonRect = position;
            buttonRect.x = textFieldRect.xMax + 5;
            buttonRect.width = 55;

            bool isGuiEnabled = GUI.enabled;
            GUI.enabled = false;

            EditorGUI.PropertyField(textFieldRect, property, label);

            GUI.enabled = isGuiEnabled;

            if (GUI.Button(buttonRect, "Browse"))
            {
                string currentPath = property.stringValue;
                string newPath = EditorUtility.OpenFolderPanel("Select Folder", currentPath, "");
                string assetsFolderPath = Application.dataPath;

                if (!newPath.StartsWith(assetsFolderPath))
                {
                    EditorUtility.DisplayDialog("C4G", "Pick folder inside Assets folder", "Ok");
                }
                else
                {
                    property.stringValue = newPath.Length == assetsFolderPath.Length
                        ? string.Empty
                        : newPath.Substring(assetsFolderPath.Length + 1);
                }
            }
        }
    }
}