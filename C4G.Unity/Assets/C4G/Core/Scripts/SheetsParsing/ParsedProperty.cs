namespace C4G.Core.SheetsParsing
{
    public readonly struct ParsedProperty
    {
        public readonly string Name;
        public readonly string Type;

        public ParsedProperty(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}