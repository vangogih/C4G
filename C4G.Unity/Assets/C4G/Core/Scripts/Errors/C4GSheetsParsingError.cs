namespace C4G.Core.Errors
{
    public sealed class C4GSheetsParsingError : C4GErrorBase
    {
        public C4GSheetsParsingError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
