namespace C4G.Core.SheetsParsing._1_PropertiesHierarchyTraversal
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