using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace C4G.Runtime.Scripts
{
    public sealed class RuntimeTest : MonoBehaviour
    {
        private void Awake()
        {
            string configPath = Path.Combine(Application.streamingAssetsPath, "Sheet1.json");
            string configText = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<Sheet1Wrapper>(configText);
            Debug.Log(config.Name);
            Debug.Log(config.Entities.Count);
        }
    }
}