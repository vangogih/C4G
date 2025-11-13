using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace C4G.Core.Utils
{
    internal static class StackTraceUtility
    {
        private const string ASSETS_FOLDER_NAME = "Assets";

        public static string GetAssetPathsOnly(int skipFrames)
        {
            if (skipFrames < 0)
                skipFrames = 0;

            var stackTrace = new StackTrace(skipFrames + 1, fNeedFileInfo: true);
            var result = new StringBuilder();

            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();

                if (string.IsNullOrEmpty(fileName) || lineNumber < 1)
                    continue;

                fileName = fileName.Replace('\\', '/');

                int assetsIndex = fileName.IndexOf(ASSETS_FOLDER_NAME, StringComparison.Ordinal);
                if (assetsIndex >= 0)
                    fileName = fileName.Substring(assetsIndex);

                MethodBase method = frame.GetMethod();
                string methodName = $"{method?.DeclaringType?.Name ?? "Unknown type"}.{method?.Name ?? "Unknown method"}";

                result.AppendLine($"{methodName} () (at {fileName}:{lineNumber})");
            }

            return result.ToString();
        }
    }
}