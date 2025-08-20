using System;
using System.IO;
using C4G.Core.Utils;

namespace C4G.Core.IO
{
    public class IOFacade
    {
        public Result<EC4GError> WriteToFiles(
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
            catch (Exception)
            {
                return Result<EC4GError>.FromError(EC4GError.IOException);
            }

            return Result<EC4GError>.Ok;
        }
    }
}