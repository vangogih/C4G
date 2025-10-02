using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public partial class ConfigsSerializationTests
    {
        private Core.ConfigsSerialization _configSerialization;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configSerialization = new Core.ConfigsSerialization();
        }
    }
}