using System;
using System.IO;
using C4G.Core.Utils;

namespace C4G.Core.IO
{
    public class IO
    {
        public Result<string> WriteToFile(string folderPath, string fileName, string fileContents)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string dtoClassFilePath = Path.Combine(folderPath, fileName);
                File.WriteAllText(dtoClassFilePath, fileContents);
            }
            catch (Exception e)
            {
                return Result<string>.FromError($"IO error. {e}");
            }

            return Result<string>.Ok;
        }
    }
}