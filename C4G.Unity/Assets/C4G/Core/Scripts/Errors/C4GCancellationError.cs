using System.Diagnostics.CodeAnalysis;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public sealed class C4GCancellationError : C4GErrorBase
    {
        public C4GCancellationError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
