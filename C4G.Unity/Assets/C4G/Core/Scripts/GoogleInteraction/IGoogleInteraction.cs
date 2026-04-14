using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core.Utils;

namespace C4G.Core.GoogleInteraction
{
	public interface IGoogleInteraction
	{
		Task<Result<IList<IList<object>>, string>> LoadSheetAsync(string sheetName, string tableId, string clientSecret, CancellationToken ct);
	}
}
