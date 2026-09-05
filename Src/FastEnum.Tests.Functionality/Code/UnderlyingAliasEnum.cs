namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
internal enum UnderlyingAliasEnum
{
    None = 0,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    Omitted = 1,
    First = Omitted,
    Second = Omitted,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    Excluded = 2
}