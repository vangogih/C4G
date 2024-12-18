using System.Collections;
using System.Collections.Generic;
using System.IO;
using C4G.Editor;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace C4G.Tests.Editor
{
    public class IntegrationTests
    {
        private const string TableId = "1xNbXhj706f2ZLUCqROPkAEki_gRe1CPomkPoWua4ogU";

        private static readonly string SecretKeyFilePath =
            Path.Combine(Constants.SecretsDirectoryPath, "c4g-test-440907-683f7e74850a.json");

        private static readonly string GeneratedFilesDirectoryPath = Path.GetFullPath(Path.Combine(Application.dataPath, $"TMP_Generated_By_{nameof(IntegrationTests)}"));

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Directory.Exists(GeneratedFilesDirectoryPath))
                Directory.Delete(GeneratedFilesDirectoryPath, true);
        }

        [UnityTest]
        public IEnumerator Test()
        {
            const string sheetName = "Sheet1";

            GoogleCredential googleCredential = GoogleCredential.FromFile(SecretKeyFilePath)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = googleCredential,
                ApplicationName = "C4G"
            });

            var request = service.Spreadsheets.Values.Get(TableId, sheetName);
            IList<IList<object>> rows = request.Execute().Values;

            ParsedSheet parsedSheet = SheetParser.ParseSheet(sheetName, rows);

            CreateJsonFile(parsedSheet);
            GenerateCode(parsedSheet);

            AssetDatabase.Refresh();

            yield return new WaitForDomainReload();
            
            Assert.IsTrue(Directory.Exists(GeneratedFilesDirectoryPath));
            Assert.IsTrue(File.Exists(Path.Combine(GeneratedFilesDirectoryPath, $"{sheetName}.json")));
            Assert.IsTrue(File.Exists(Path.Combine(GeneratedFilesDirectoryPath, $"{sheetName}.cs")));
            Assert.IsTrue(File.Exists(Path.Combine(GeneratedFilesDirectoryPath, $"{sheetName}Wrapper.cs")));
        }

        private static void CreateJsonFile(ParsedSheet parsedSheet)
        {
            string json = ParsedSheetToJsonConverter.ConvertParsedSheetToJson(parsedSheet);

            if (!Directory.Exists(GeneratedFilesDirectoryPath))
            {
                Directory.CreateDirectory(GeneratedFilesDirectoryPath);
            }

            string filePath = Path.Combine(GeneratedFilesDirectoryPath, $"{parsedSheet.Name}.json");

            File.WriteAllText(filePath, json);
        }

        private static void GenerateCode(ParsedSheet parsedSheet)
        {
            string dtoClass = CodeGenerator.GenerateDTOClass(parsedSheet);
            string wrapperClass = CodeGenerator.GenerateWrapperClass(parsedSheet);
            if (!Directory.Exists(GeneratedFilesDirectoryPath))
                Directory.CreateDirectory(GeneratedFilesDirectoryPath);
            string dtoClassFilePath = Path.Combine(GeneratedFilesDirectoryPath, $"{parsedSheet.Name}.cs");
            File.WriteAllText(dtoClassFilePath, dtoClass);
            string wrapperClassFilePath = Path.Combine(GeneratedFilesDirectoryPath, $"{parsedSheet.Name}Wrapper.cs");
            File.WriteAllText(wrapperClassFilePath, wrapperClass);
        }
    }
}