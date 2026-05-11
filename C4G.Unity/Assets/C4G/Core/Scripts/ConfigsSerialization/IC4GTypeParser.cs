using System;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization
{
    public interface IC4GTypeParser
    {
        Type ParsingType { get; }
        Result<object, C4GConfigsSerializationError> Parse(string value);
    }
}

