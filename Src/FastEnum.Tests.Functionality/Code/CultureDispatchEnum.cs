namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
internal enum CultureDispatchEnum
{
    [EnumTransformValue(ValueOverride = "coop")]
    First,
    [EnumTransformValue(ValueOverride = "co-op")]
    Second
}