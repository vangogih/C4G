using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace C4G.Core.GoogleInteraction
{
	[ExcludeFromCodeCoverage]
	public sealed class GoogleInteraction : IGoogleInteraction
	{
		public async Task<Result<IList<IList<object>>, string>> LoadSheetAsync(string sheetName, string tableId, string clientSecret, CancellationToken ct)
		{
			if (ct.IsCancellationRequested)
				return Result<IList<IList<object>>, string>.FromError("Google interaction. Task cancelled");

			var clientSecretBytes = Encoding.UTF8.GetBytes(clientSecret);
			var clientSecretMemoryStream = new MemoryStream(clientSecretBytes);
			GoogleClientSecrets googleClientSecrets = await GoogleClientSecrets.FromStreamAsync(clientSecretMemoryStream, ct);
			var dataStore = new FileDataStore("C4G");

			UserCredential credential = await AuthorizeAsync(googleClientSecrets, dataStore, ct);

			try
			{
				return await ExecuteSheetRequestAsync(credential, tableId, sheetName, ct);
			}
			catch (TokenResponseException)
			{
				await dataStore.ClearAsync();
				credential = await AuthorizeAsync(googleClientSecrets, dataStore, ct);
				return await ExecuteSheetRequestAsync(credential, tableId, sheetName, ct);
			}
		}

		private static async Task<UserCredential> AuthorizeAsync(GoogleClientSecrets secrets, FileDataStore dataStore, CancellationToken ct)
		{
			return await GoogleWebAuthorizationBroker.AuthorizeAsync(
				secrets.Secrets,
				new[] { SheetsService.Scope.SpreadsheetsReadonly },
				"user",
				ct,
				dataStore);
		}

		private static async Task<Result<IList<IList<object>>, string>> ExecuteSheetRequestAsync(UserCredential credential, string tableId, string sheetName, CancellationToken ct)
		{
			var sheetsService = new SheetsService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = "c4g-test-440907"
			});

			SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(tableId, sheetName);
			ValueRange response = await request.ExecuteAsync(ct);

			return Result<IList<IList<object>>, string>.FromValue(response.Values);
		}
	}
}