namespace C4G.Core.Errors
{
    public sealed class C4GConfigsSerializationError : C4GErrorBase
    {
        public C4GConfigsSerializationError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
