using UnityEditor;
using UnityEngine;
using System.IO;

namespace C4G.Editor
{
    internal class FolderReferenceAttribute : PropertyAttribute {}

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
                string projectFolderPath = GetProjectFolderPath();
                string folderRelativePath = property.stringValue;

                (string parentFolderRelativePath, string folderName) = SplitFolderRelativePathForDialog(projectFolderPath, folderRelativePath);

                string selectedFolderPath = EditorUtility.OpenFolderPanel("Select Folder", parentFolderRelativePath, folderName);

                if (string.IsNullOrEmpty(selectedFolderPath))
                    return;

                if (!selectedFolderPath.StartsWith(projectFolderPath))
                {
                    EditorUtility.DisplayDialog("C4G", "Selected folder must be inside project", "Ok");
                }
                else
                {
                    property.stringValue = selectedFolderPath.Length == projectFolderPath.Length
                        ? string.Empty
                        : selectedFolderPath.Substring(projectFolderPath.Length + 1);
                }
            }
        }

        private static (string parentFolderRelativePath, string folderName) SplitFolderRelativePathForDialog(
            string projectFolderPath, string folderRelativePath)
        {
            if (string.IsNullOrWhiteSpace(folderRelativePath) || folderRelativePath.IndexOf('\\') != -1)
                return (parentFolderRelativePath: string.Empty, folderName: string.Empty);

            string folderPath = $"{projectFolderPath}/{folderRelativePath}";

            if (!Directory.Exists(folderPath))
                return (parentFolderRelativePath: string.Empty, folderName: string.Empty);

            string cleanFolderRelativePath = folderRelativePath.TrimEnd('/');
            int lastSlashIndex = cleanFolderRelativePath.LastIndexOf('/');

            if (lastSlashIndex == -1)
                return (parentFolderRelativePath: string.Empty, folderName: cleanFolderRelativePath);

            string parentFolderRelativePath = cleanFolderRelativePath.Substring(0, lastSlashIndex);
            string folderName = cleanFolderRelativePath.Substring(lastSlashIndex + 1);

            return (parentFolderRelativePath, folderName);
        }

        private static string GetProjectFolderPath()
        {
            const int assetsFolderSubstringLength = 7;
            int pathLengthWithoutAssetsSubstring = Application.dataPath.Length - assetsFolderSubstringLength;
            string projectFolderPath = Application.dataPath.Substring(0, pathLengthWithoutAssetsSubstring);
            return projectFolderPath;
        }
    }
}