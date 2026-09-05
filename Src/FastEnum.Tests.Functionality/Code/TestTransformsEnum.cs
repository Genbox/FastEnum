namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
[EnumTransform(Preset = EnumTransform.UpperCase)]
public enum TestTransformsEnum : long
{
    [EnumTransformValue(ValueOverride = "ThisWasOverriden")]
    OverrideMe,
    uppercase
}