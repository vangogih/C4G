using C4G.Core.ConfigsSerialization;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public partial class ConfigsSerializerTests
    {
        private ConfigsSerializer _configSerializer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configSerializer = new ConfigsSerializer();
        }
    }
}