using System.IO;
using UnityEngine;

namespace C4G.Editor
{
    public static class Constants
    {
        public static readonly string SecretsDirectoryPath =
            Path.GetFullPath(Path.Combine(Application.dataPath, "C4G", "Editor", "Secrets"));
    }
}