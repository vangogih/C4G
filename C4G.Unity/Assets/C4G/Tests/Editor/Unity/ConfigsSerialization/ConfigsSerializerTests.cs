using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using NUnit.Framework;

namespace C4G.Tests.Editor.Unity.ConfigsSerialization
{
    [TestFixture]
    public partial class ConfigsSerializerTests
    {
        private ConfigsSerializer _configsSerializer;
        private Dictionary<string, IC4GTypeParser> _parsersByName;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _configsSerializer = new ConfigsSerializer();
            _parsersByName = new Dictionary<string, IC4GTypeParser>();
        }

        [SetUp]
        public void SetUp()
        {
            _parsersByName.Clear();
        }
    }
}