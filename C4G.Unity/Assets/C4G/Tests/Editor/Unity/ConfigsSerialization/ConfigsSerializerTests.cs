using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    [TestFixture]
    public partial class ConfigsSerializerTests
    {
        private ConfigsSerializer _configsSerializer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configsSerializer = new ConfigsSerializer(new List<AliasDefinition>());
        }
    }
}