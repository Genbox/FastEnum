namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
[Flags]
internal enum FullyOmittedFlags
{
    [EnumOmitValue]
    None = 0,
    [EnumOmitValue]
    First = 1
}