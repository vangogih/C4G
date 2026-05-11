namespace C4G.Core.Errors
{
    public sealed class C4GSettingsError : C4GErrorBase
    {
        public C4GSettingsError(string message, C4GErrorBase cause = null) : base(message, cause) { }
    }
}
