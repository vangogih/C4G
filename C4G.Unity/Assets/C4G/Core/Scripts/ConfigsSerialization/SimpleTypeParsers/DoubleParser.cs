using System;
using System.Globalization;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization.SimpleTypeParsers
{
    [Serializable]
    internal sealed class DoubleParser : IC4GTypeParser
    {
        Type IC4GTypeParser.ParsingType { get; } = typeof(double);

        Result<object, C4GConfigsSerializationError> IC4GTypeParser.Parse(string value)
        {
            return double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double doubleValue)
                ? Result<object, C4GConfigsSerializationError>.FromValue(doubleValue)
                : Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError($"Could not parse '{value}' as double", null));
        }
    }
}