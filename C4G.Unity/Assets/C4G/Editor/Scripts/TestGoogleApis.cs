using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEditor;
using UnityEngine;

namespace C4G.Editor
{
    public static class TestGoogleApis
    {
        private const string TableId = "1xNbXhj706f2ZLUCqROPkAEki_gRe1CPomkPoWua4ogU";

        [MenuItem("Tools/C4G/Test")]
        public static void Test()
        { 
            Debug.Log("Test");
            GoogleCredential googleCredential = GoogleCredential.FromFile(Path.GetFullPath(Path.Combine(Application.dataPath,
                "C4G", "Editor", "Secrets", "c4g-test-440907-683f7e74850a.json"))).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly); 
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = googleCredential,
                ApplicationName = "C4G"
            });
            var request = service.Spreadsheets.Values.Get(TableId, "Sheet1");
            IList<IList<object>> rows = request.Execute().Values;
            foreach (var objects in rows)
            {
                Debug.Log(string.Join(", ", objects));
            }
        }
    }
}