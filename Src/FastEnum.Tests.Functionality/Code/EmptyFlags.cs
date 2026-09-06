namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum, Flags]
internal enum EmptyFlags { }

[FastEnum, Flags]
internal enum OmittedNonzeroFlags
{
    [EnumOmitValue]
    First = 1
}