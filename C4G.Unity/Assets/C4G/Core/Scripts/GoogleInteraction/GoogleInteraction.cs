using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using C4G.Core.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace C4G.Core.GoogleInteraction
{
    public sealed class GoogleInteraction
    {
        public async Task<Result<IList<IList<object>>, string>> LoadSheetAsync(string sheetName, string tableId, string clientSecret, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<IList<IList<object>>, string>.FromError("Google interaction. Task cancelled");

            byte[] clientSecretBytes = Encoding.UTF8.GetBytes(clientSecret);
            MemoryStream clientSecretMemoryStream = new MemoryStream(clientSecretBytes);
            GoogleClientSecrets googleClientSecrets = await GoogleClientSecrets.FromStreamAsync(clientSecretMemoryStream, ct);
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                googleClientSecrets.Secrets,
                new[] { SheetsService.Scope.SpreadsheetsReadonly },
                "user",
                CancellationToken.None,
                new FileDataStore("C4G"));

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