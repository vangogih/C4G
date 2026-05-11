namespace C4G.Core.Errors
{
    public sealed class C4GCodeGenerationError : C4GErrorBase
    {
        public C4GCodeGenerationError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
