namespace Genbox.FastEnum.Tests.Functionality.Code;

[Flags]
[FastEnum]
internal enum OmittedCompositeFlagsEnum
{
    First = 1,
    Second = 2,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    Both = First | Second
}