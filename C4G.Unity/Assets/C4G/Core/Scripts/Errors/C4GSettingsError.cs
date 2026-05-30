using System.Diagnostics.CodeAnalysis;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public sealed class C4GSettingsError : C4GErrorBase
    {
        public C4GSettingsError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
