using System;
using System.Globalization;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class DoubleParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } = typeof(double);

        Result<object, string> IC4GTypeParser.Parse(string value)
        {
            return double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double doubleValue)
                ? Result<object, string>.FromValue(doubleValue)
                : Result<object, string>.FromError($"Could not parse '{value}' as double");
        }
    }
}