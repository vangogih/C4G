using System.Diagnostics.CodeAnalysis;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public sealed class C4GGoogleInteractionError : C4GErrorBase
    {
        public C4GGoogleInteractionError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
