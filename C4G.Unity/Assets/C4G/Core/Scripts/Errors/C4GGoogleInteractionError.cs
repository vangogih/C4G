namespace C4G.Core.Errors
{
    public sealed class C4GGoogleInteractionError : C4GErrorBase
    {
        public C4GGoogleInteractionError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
