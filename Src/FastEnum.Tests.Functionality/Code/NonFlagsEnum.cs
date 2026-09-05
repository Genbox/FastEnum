namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
public enum NonFlagsEnum : ulong
{
    Value1,
    Value2,
    Max = ulong.MaxValue
}