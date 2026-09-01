// DisableEnumsWrapper should remove the Enums wrapper

[FastEnum(DisableEnumsWrapper = true, EnumsClassNamespace = "Enums")]
public enum MyEnum
{
    Value
}