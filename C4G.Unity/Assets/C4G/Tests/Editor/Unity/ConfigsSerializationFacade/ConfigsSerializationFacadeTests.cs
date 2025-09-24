using NUnit.Framework;

namespace C4G.Tests.Editor.Unity
{
    public partial class ConfigsSerializationFacadeTests
    {
        protected Core.ConfigsSerializationFacade ConfigSerializationFacade { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ConfigSerializationFacade = new Core.ConfigsSerializationFacade();
        }
    }
}