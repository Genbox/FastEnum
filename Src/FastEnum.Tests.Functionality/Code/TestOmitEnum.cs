namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
public enum TestOmitEnum
{
    [EnumOmitValue]
    Omitted,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
    OmittedWithFilter
}