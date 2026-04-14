using C4G.Core.Utils;
using C4G.Unity.Assets.C4G.Core.Scripts.Utils;

namespace C4G.Core.Settings
{
    public interface IC4GSettingsProvider
    {
        Result<C4GSettings, C4GError> GetSettings();
    }
}