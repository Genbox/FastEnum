namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
[Flags]
internal enum OmittedValidationFlags : long
{
    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
    None = 0,
    First = 1,
    Alias = First,
    Second = 2,
    Third = 4,
    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined | EnumOmitExclude.TryGetUnderlyingValue)]
    Both = First | Second,
    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined | EnumOmitExclude.TryGetUnderlyingValue)]
    BothAlias = Both,
    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined | EnumOmitExclude.TryGetUnderlyingValue)]
    OmittedThird = Third,
    Sign = long.MinValue,
    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
    SignedFirst = Sign | First
}