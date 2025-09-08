namespace C4G.Core
{
    public readonly struct C4GError
    {
        public static C4GError TaskCancelled = new C4GError(EC4GErrorType.TaskCancelled, string.Empty);

        public readonly EC4GErrorType Type;
        public readonly string Message;

        public C4GError(EC4GErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}