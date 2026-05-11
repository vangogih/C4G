using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.Settings
{
    public interface IC4GSettingsProvider
    {
        Result<C4GSettings, C4GSettingsError> GetSettings();
    }
}