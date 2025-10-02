using System;
using System.IO;
using C4G.Core.Utils;

namespace C4G.Core.IO
{
    public class IO
    {
        public Result<string> WriteToFiles(
            string codeFolderFullPath, string dtoClassFileName, string dtoClass, string wrapperClassFileName, string wrapperClass,
            string configsFolderFullPath, string configsFileName, string configs)
        {
            try
            {
                if (!Directory.Exists(codeFolderFullPath))
                    Directory.CreateDirectory(codeFolderFullPath);

                string dtoClassFilePath = Path.Combine(codeFolderFullPath, dtoClassFileName);
                File.WriteAllText(dtoClassFilePath, dtoClass);

                string wrapperClassFilePath =
                    Path.Combine(codeFolderFullPath, wrapperClassFileName);
                File.WriteAllText(wrapperClassFilePath, wrapperClass);

                if (!Directory.Exists(configsFolderFullPath))
                    Directory.CreateDirectory(configsFolderFullPath);

                string jsonFilePath = Path.Combine(configsFolderFullPath, configsFileName);

                File.WriteAllText(jsonFilePath, configs);
            }
            catch (Exception e)
            {
                return Result<string>.FromError($"IO error. {e}");
            }

            return Result<string>.Ok;
        }
    }
}