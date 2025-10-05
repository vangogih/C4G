using System.Globalization;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    internal sealed class FloatParser : IC4GTypeParser
    {
        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float floatValue)
                ? Result<object, string>.FromValue(floatValue)
                : Result<object, string>.FromError($"Could not parse '{value}' as float");
        }
    }
}