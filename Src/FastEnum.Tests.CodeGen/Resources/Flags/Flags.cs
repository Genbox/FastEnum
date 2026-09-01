// Flag enum support

namespace Some.Namespace.Here;

[FastEnum]
[Flags]
public enum MyEnum
{
    First = 0,
    Second = 2,
    Third = 8,
    Other = 256,
}