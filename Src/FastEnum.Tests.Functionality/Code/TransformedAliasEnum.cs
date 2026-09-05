namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
[EnumTransform(Preset = EnumTransform.LowerCase)]
internal enum TransformedAliasEnum
{
    None = 0,
    First = 1,
    Alias = First
}