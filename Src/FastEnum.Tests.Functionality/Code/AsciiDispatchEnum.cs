namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
internal enum AsciiDispatchEnum
{
    K,
    S,
    I,
    [EnumTransformValue(ValueOverride = "i")]
    LowerI
}