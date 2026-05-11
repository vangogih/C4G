using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.IO
{
	public interface IIO
	{
		Result<C4GIOError> WriteToFile(string folderPath, string fileName, string fileContents);
	}
}
