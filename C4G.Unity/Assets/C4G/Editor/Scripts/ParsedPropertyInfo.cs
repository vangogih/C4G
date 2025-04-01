namespace C4G.Editor
{
    public sealed class ParsedPropertyInfo
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