// Omitted composite flag support

namespace Some.Namespace.Here;

[FastEnum]
[Flags]
public enum MyEnum
{
    First = 1,
    Second = 2,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue | EnumOmitExclude.IsDefined)]
    Both = First | Second
}