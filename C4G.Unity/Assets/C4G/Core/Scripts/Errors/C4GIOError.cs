using System.Diagnostics.CodeAnalysis;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public sealed class C4GIOError : C4GErrorBase
    {
        public C4GIOError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
