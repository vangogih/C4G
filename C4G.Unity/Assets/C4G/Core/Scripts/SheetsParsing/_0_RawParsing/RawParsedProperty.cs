namespace C4G.Core.SheetsParsing._0_RawParsing
{
    public readonly struct RawParsedProperty
    {
        public readonly string NameWithPossibleHierarchy;
        public readonly string Type;

        public RawParsedProperty(string nameWithPossibleHierarchy, string type)
        {
            NameWithPossibleHierarchy = nameWithPossibleHierarchy;
            Type = type;
        }
    }
}