using System;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class StringParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } =  typeof(string);

        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return Result<object, string>.FromValue(value);
        }
    }
}