using System.Collections.Generic;

namespace C4G.Core.SheetsParsing
{
    public readonly struct ParsedConfig
    {
        public readonly string Name;
        public readonly IReadOnlyList<ParsedPropertyInfo> Properties;
        public readonly IReadOnlyList<IReadOnlyCollection<string>> Entities;

        public ParsedConfig(string name, IReadOnlyList<ParsedPropertyInfo> properties, IReadOnlyList<IReadOnlyCollection<string>> entities)
        {
            Name = name;
            Properties = properties;
            Entities = entities;
        }
    }
}