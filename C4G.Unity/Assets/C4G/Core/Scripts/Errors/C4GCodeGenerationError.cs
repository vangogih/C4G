using System.Diagnostics.CodeAnalysis;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public sealed class C4GCodeGenerationError : C4GErrorBase
    {
        public C4GCodeGenerationError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
