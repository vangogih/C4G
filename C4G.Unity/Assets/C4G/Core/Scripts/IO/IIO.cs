using C4G.Core.Utils;

namespace C4G.Core.IO
{
	public interface IIO
	{
		Result<string> WriteToFile(string folderPath, string fileName, string fileContents);
	}
}
