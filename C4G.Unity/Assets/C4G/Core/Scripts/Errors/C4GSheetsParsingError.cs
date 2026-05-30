using System.Diagnostics.CodeAnalysis;

namespace C4G.Core.Errors
{
    [ExcludeFromCodeCoverage]
    public sealed class C4GSheetsParsingError : C4GErrorBase
    {
        public C4GSheetsParsingError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
