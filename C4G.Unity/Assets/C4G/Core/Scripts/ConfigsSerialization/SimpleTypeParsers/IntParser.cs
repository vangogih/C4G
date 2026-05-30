using System;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class IntParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } = typeof(int);

        Result<object, C4GConfigsSerializationError> IC4GTypeParser.Parse(string value)
        {
            return int.TryParse(value, out int intValue)
                ? Result<object, C4GConfigsSerializationError>.FromValue(intValue)
                : Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError($"Could not parse '{value}' as int", null));
        }
    }
}