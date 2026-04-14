namespace C4G.Unity.Assets.C4G.Core.Scripts.Utils
{
    public abstract class C4GError
    {
        public readonly string Message;

        public C4GError(string message)
        {
            Message = message;
        }

        public sealed class Settings : C4GError
        {
            public Settings(string message) : base(message) {}
        }

        public sealed class Cancelled : C4GError
        {
            public Cancelled(string message) : base(message) {}

            public static readonly Cancelled Instance = new Cancelled("Operation was cancelled"); 
        }

        public sealed class SheetsParsing : C4GError
        {
            public readonly string SheetName;
            public readonly int Row = -1;
            public readonly int Column = -1;

            public SheetsParsing(string message) : base(message) {}

            public SheetsParsing(string message, string sheetName) : base(message)
            {
                SheetName = sheetName;
            }

            public SheetsParsing(string message, string sheetName, int row, int column) : base(message)
            {
                SheetName = sheetName;
                Row = row;
                Column = column;
            }
        }
    }
}