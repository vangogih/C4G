using System.Collections.Generic;

namespace C4G.Core.SheetsParsing
{
    public readonly struct ParsedSheet
    {
        public readonly string Name;
        public readonly IReadOnlyCollection<ParsedPropertyInfo> Properties;
        public readonly IReadOnlyCollection<IReadOnlyCollection<string>> Entities;

        public ParsedSheet(string name, IReadOnlyCollection<ParsedPropertyInfo> properties, IReadOnlyCollection<IReadOnlyCollection<string>> entities)
        {
            Name = name;
            Properties = properties;
            Entities = entities;
        }
    }
}