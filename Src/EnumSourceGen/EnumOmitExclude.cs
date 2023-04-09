namespace Genbox.EnumSourceGen;

[Flags]
public enum EnumOmitExclude
{
    None = 0,
    GetMemberNames = 1,
    GetMemberValues = 2,
    GetUnderlyingValue = 4,
    Parse = 8,
    GetDisplayName = 16,
    GetDescription = 32,
    IsDefined = 64,
    GetString = 128,
    GetValue = 256,
    All = int.MaxValue
}