using System.Globalization;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    internal sealed class DoubleParser : IC4GTypeParser
    {
        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double doubleValue)
                ? Result<object, string>.FromValue(doubleValue)
                : Result<object, string>.FromError($"Could not parse '{value}' as double");
        }
    }
}