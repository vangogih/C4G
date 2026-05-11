namespace C4G.Core.Errors
{
    public sealed class C4GIOError : C4GErrorBase
    {
        public C4GIOError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
