using System.Collections.Generic;

namespace C4G.Core.SheetsParsing
{
    public readonly struct ParsedConfig
    {
        public readonly string Name;
        public readonly ParsedPropertyInfo[] Properties;
        public readonly List<string> SubTypes;
        public readonly IReadOnlyList<IReadOnlyCollection<string>> Entities;

        public ParsedConfig(string name, ParsedPropertyInfo[] properties, IReadOnlyList<IReadOnlyCollection<string>> entities)
        {
            Name = name;
            Properties = properties;
            SubTypes = new List<string>();
            Entities = entities;
        }
    }
}