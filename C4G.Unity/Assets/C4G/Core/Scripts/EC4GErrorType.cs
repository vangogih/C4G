namespace C4G.Core
{
    public enum EC4GErrorType : byte
    {
        TaskCancelled = 0,
        GoogleInteraction = 1,
        SheetsParsing = 2,
        CodeGeneration = 3,
        ConfigsSerialization = 4,
        IO = 5
    }
}