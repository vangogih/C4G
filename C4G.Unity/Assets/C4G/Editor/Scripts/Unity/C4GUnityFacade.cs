using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using C4G.Editor.Core;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace C4G.Editor.Unity
{
    public sealed class C4GUnityFacade : IC4GFacade
    {
        private readonly string _clientSecret;
        private readonly string _tableId;
        private readonly string _sheetName;

        public C4GUnityFacade(string tableId, string sheetName, string clientSecret)
        {
            _tableId = tableId;
            _sheetName = sheetName;
            _clientSecret = clientSecret;
        }

        public async Task<IList<IList<object>>> LoadRawConfigAsync(CancellationToken ct)
        {
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(_clientSecret)), ct)).Secrets,
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
            return (await request.ExecuteAsync(ct)).Values;
        }

        public ParsedSheet ParseSheet(string sheetName, IList<IList<object>> rawConfig)
            => SheetParser.ParseSheet(sheetName, rawConfig);

        public string ConvertParsedSheetToJsonString(ParsedSheet parsedSheet)
            => ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

        public string GenerateDTOClassFromParsedSheet(ParsedSheet parsedSheet)
            => CodeGenerator.GenerateDTOClass(parsedSheet);

        public string GenerateWrapperClassFromParsedSheet(ParsedSheet parsedSheet)
            => CodeGenerator.GenerateWrapperClass(parsedSheet);

        public void GenerateJsonConfigsAndWriteToFolder(ParsedSheet parsedSheet, string folderPath)
        {
            string json = ConvertParsedSheetToJsonString(parsedSheet);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{parsedSheet.Name}.json");

            File.WriteAllText(filePath, json);
        }

        public void GenerateCodeAndWriteToFolder(ParsedSheet parsedSheet, string folderPath)
        {
            string dtoClass = GenerateDTOClassFromParsedSheet(parsedSheet);
            string wrapperClass = GenerateWrapperClassFromParsedSheet(parsedSheet);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string dtoClassFilePath = Path.Combine(folderPath, $"{parsedSheet.Name}.cs");
            File.WriteAllText(dtoClassFilePath, dtoClass);

            string wrapperClassFilePath = Path.Combine(folderPath, $"{parsedSheet.Name}Wrapper.cs");
            File.WriteAllText(wrapperClassFilePath, wrapperClass);
        }
    }
}