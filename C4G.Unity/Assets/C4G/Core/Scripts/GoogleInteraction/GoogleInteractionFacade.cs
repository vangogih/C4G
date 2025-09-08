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
    public sealed class GoogleInteractionFacade
    {
        private readonly string _tableId;
        private readonly string _sheetName;
        private readonly string _clientSecret;

        public GoogleInteractionFacade(string tableId, string sheetName, string clientSecret)
        {
            _tableId = tableId;
            _sheetName = sheetName;
            _clientSecret = clientSecret;
        }

        public async Task<Result<IList<IList<object>>, C4GError>> LoadRawConfigAsync(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Result<IList<IList<object>>, C4GError>.FromError(C4GError.TaskCancelled);

            byte[] clientSecretBytes = Encoding.UTF8.GetBytes(_clientSecret);
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

            SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(_tableId, _sheetName);
            ValueRange response = await request.ExecuteAsync(ct);

            return Result<IList<IList<object>>, C4GError>.FromValue(response.Values);
        }
    }
}