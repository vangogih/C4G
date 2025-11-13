using C4G.Core.Utils;

namespace C4G.Core.Settings
{
    public interface IC4GSettingsProvider
    {
        Result<C4GSettings, string> GetSettings();
    }
}