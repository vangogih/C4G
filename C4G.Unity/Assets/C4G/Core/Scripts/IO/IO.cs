using System;
using System.IO;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.IO
{
    public class IO : IIO
    {
        public Result<C4GIOError> WriteToFile(string folderPath, string fileName, string fileContents)
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
                return Result<C4GIOError>.FromError(new C4GIOError($"Exception during C4G IO.\n{e}", null));
            }

            return Result<C4GIOError>.Ok;
        }
    }
}