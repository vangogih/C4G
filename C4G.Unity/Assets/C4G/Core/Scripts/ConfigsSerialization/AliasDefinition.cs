using System;

namespace C4G.Core.ConfigsSerialization
{
    public readonly struct AliasDefinition
    {
        public readonly string Alias;
        public readonly Type UnderlyingType;
        public readonly IC4GTypeParser Parser;

        public AliasDefinition(string alias, Type underlyingType, IC4GTypeParser parser)
        {
            Alias = alias;
            UnderlyingType = underlyingType;
            Parser = parser;
        }
    }
}