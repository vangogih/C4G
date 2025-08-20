namespace C4G.Core.SheetsParsing
{
    public readonly struct ParsedPropertyInfo
    {
        public readonly string Name;
        public readonly string Type;

        public ParsedPropertyInfo(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}