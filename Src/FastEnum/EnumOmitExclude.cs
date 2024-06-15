namespace Genbox.FastEnum;

[Flags]
public enum EnumOmitExclude
{
    None = 0,
    GetMemberNames = 1 << 0,
    GetMemberValues = 1 << 1,
    GetUnderlyingValues = 1 << 2,
    TryGetUnderlyingValue = 1 << 3,
    TryParse = 1 << 4,
    TryGetDisplayName = 1 << 5,
    TryGetDescription = 1 << 6,
    IsDefined = 1 << 7,
    GetString = 1 << 8,
    All = GetMemberNames | GetMemberValues | GetUnderlyingValues | TryGetUnderlyingValue | TryParse | TryGetDisplayName | TryGetDescription | IsDefined | GetString
}