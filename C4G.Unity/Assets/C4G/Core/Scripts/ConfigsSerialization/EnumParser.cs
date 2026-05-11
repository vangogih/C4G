using System;
using C4G.Core.Errors;
using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization
{
    public abstract class EnumParser<T> : IC4GTypeParser where T : Enum
    {
        private readonly Type _enumType = typeof(T);
        private readonly bool _hasFlagsAttribute;

        public EnumParser()
        {
            _hasFlagsAttribute = Attribute.IsDefined(_enumType, typeof(FlagsAttribute));
        }

        Type IC4GTypeParser.ParsingType => _enumType;

        Result<object, C4GConfigsSerializationError> IC4GTypeParser.Parse(string value)
        {
            try
            {
                object enumValue = Enum.Parse(_enumType, value);
                if (!_hasFlagsAttribute && !Enum.IsDefined(_enumType, enumValue))
                {
                    return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(
                        $"EnumParser. Value '{value}' is not defined in enum '{_enumType.Name}'. " +
                        "Either fix value or mark enum with [Flags] attribute", null));
                }

                return Result<object, C4GConfigsSerializationError>.FromValue(enumValue);
            }
            catch (Exception e)
            {
                return Result<object, C4GConfigsSerializationError>.FromError(new C4GConfigsSerializationError(
                    $"Exception during enum parsing\n{e}", null));
            }
        }
    }
}