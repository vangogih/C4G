using System.Collections.Generic;
using C4G.Core.SheetsParsing._0_RawParsing;

namespace C4G.Core.Scripts.SheetsParsing._1_PropertiesHierarchyTraversal
{
    public readonly struct ParsedConfig
    {
        public readonly string Name;
        public readonly ParsedProperty[] ParsedProperties;
        public readonly string[] ParsedNestedPropertiesTypes;
        public readonly ParsedProperty[][] ParsedNestedProperties;

    }
}