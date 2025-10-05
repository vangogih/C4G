using System;

namespace C4G.Core.Utils
{
    public static class EnumHelper
    {
        public static bool TryParse(Type enumType, string value, out object result)
        {
            result = null;

            try
            {
                result = Enum.Parse(enumType, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}