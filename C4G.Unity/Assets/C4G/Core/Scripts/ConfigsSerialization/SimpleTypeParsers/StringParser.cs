using System;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class StringParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } =  typeof(string);

        Result<object, C4GConfigsSerializationError> IC4GTypeParser.Parse(string value)
        {
            return Result<object, C4GConfigsSerializationError>.FromValue(value);
        }
    }
}