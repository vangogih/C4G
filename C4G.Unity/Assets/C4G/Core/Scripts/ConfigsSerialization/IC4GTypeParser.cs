using C4G.Core.Utils;

namespace C4G.Core.ConfigsSerialization
{
    public interface IC4GTypeParser
    {
        Result<object, string> Parse(string value);
    }
}

