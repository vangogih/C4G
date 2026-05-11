namespace C4G.Core.Errors
{
    public sealed class C4GCancellationError : C4GErrorBase
    {
        public C4GCancellationError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
