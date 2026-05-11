namespace C4G.Core.SheetsParsing
{
    public struct ParsedPropertyInfo
    {
        public string Name;
        public string Type;
        public int SubTypeIndex;

        public ParsedPropertyInfo(string name, string type)
        {
            Name = name;
            Type = type;
            SubTypeIndex = -1;
        }
    }
}