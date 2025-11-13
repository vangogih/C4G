using System;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization
{
    public interface IC4GTypeParser
    {
        Type ParsingType { get; }
        Result<object, string> Parse(string value);
    }
}

