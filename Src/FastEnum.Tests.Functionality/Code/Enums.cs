using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code
{
    [FastEnum]
    public enum NonFlagsEnum : ulong
    {
        Value1,
        Value2,
        Max = ulong.MaxValue
    }

    [Flags]
    [FastEnum]
    public enum TestEnum : long
    {
        [Display(Name = "FirstDisplayName", Description = "FirstDescription")]
        First = 8,
        Second = 1,
        Third = 2,
        Other = 256,
        Min = long.MinValue
    }

    [FastEnum]
    [EnumTransform(Preset = EnumTransform.UpperCase)]
    public enum TestTransformsEnum : long
    {
        [EnumTransformValue(ValueOverride = "ThisWasOverriden")]
        OverrideMe,
        uppercase
    }

    [FastEnum]
    public enum TestOmitEnum
    {
        [EnumOmitValue]
        Omitted,
        [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
        OmittedWithFilter
    }
}

namespace Genbox.FastEnum.Tests.Functionality.OtherNamespace
{
    [FastEnum]
    public enum TestEnum
    {
        First,
        Second,
        Third
    }
}