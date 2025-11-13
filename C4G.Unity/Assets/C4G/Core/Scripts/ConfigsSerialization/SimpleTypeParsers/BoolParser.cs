using System;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class BoolParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } = typeof(bool);

        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return bool.TryParse(value, out bool boolValue)
                ? Result<object, string>.FromValue(boolValue)
                : Result<object, string>.FromError($"Could not parse '{value}' as bool");
        }
    }
}