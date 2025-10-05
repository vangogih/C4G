using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization.ConfigsSerializer
{
    [TestFixture]
    public partial class ConfigsSerializerTests
    {
        private Core.ConfigsSerialization.ConfigsSerializer _configsSerializer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configsSerializer = new Core.ConfigsSerialization.ConfigsSerializer();
        }
    }
}