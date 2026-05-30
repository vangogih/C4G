using System;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class BoolParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } = typeof(bool);

        Result<object, C4GConfigsSerializationError> IC4GTypeParser.Parse(string value)
        {
            return bool.TryParse(value, out bool boolValue)
                ? Result<object, C4GConfigsSerializationError>.FromValue(boolValue)
                : Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError($"Could not parse '{value}' as bool", null));
        }
    }
}