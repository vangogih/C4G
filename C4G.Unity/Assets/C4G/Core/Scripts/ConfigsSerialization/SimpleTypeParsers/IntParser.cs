using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    internal sealed class IntParser : IC4GTypeParser
    {
        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return int.TryParse(value, out int intValue)
                ? Result<object, string>.FromValue(intValue)
                : Result<object, string>.FromError($"Could not parse '{value}' as int");
        }
    }
}