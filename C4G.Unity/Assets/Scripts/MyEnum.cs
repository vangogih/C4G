using C4G.Core.ConfigsSerialization;

public enum MyEnum
{
    MyEnum0 = 0,
    MyEnum1 = 1,
    MyEnum2 = 2
}

public sealed class MyEnumParser : EnumParser<MyEnum> {}