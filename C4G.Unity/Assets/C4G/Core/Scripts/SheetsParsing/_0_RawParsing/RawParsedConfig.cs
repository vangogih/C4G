using System.Collections.Generic;

namespace C4G.Core.SheetsParsing._0_RawParsing
{
    public readonly struct RawParsedConfig
    {
        public readonly string Name;
        public readonly IReadOnlyList<RawParsedProperty> Properties;
        public readonly IReadOnlyList<IReadOnlyCollection<string>> Entities;

        public RawParsedConfig(string name, IReadOnlyList<RawParsedProperty> properties, IReadOnlyList<IReadOnlyCollection<string>> entities)
        {
            Name = name;
            Properties = properties;
            Entities = entities;
        }
    }
}