using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    internal sealed class StringParser : IC4GTypeParser
    {
        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return Result<object, string>.FromValue(value);
        }
    }
}