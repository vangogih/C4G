using System.Collections.Generic;

namespace C4G.Core.SheetsParsing._1_PropertiesHierarchyTraversal
{
    public readonly struct ParsedConfig
    {
        public readonly string Name;
        public readonly IReadOnlyList<ParsedProperty> Properties;
        public readonly IReadOnlyList<IReadOnlyCollection<string>> Entities;

        public ParsedConfig(string name, IReadOnlyList<ParsedProperty> properties, IReadOnlyList<IReadOnlyCollection<string>> entities)
        {
            Name = name;
            Properties = properties;
            Entities = entities;
        }
    }
}