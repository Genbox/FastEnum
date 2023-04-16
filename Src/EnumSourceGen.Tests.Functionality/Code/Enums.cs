using System.ComponentModel.DataAnnotations;

namespace Genbox.EnumSourceGen.Tests.Functionality.Code
{
    [EnumSourceGen]
    public enum NonFlagsEnum : ulong
    {
        Value1,
        Value2,
        Max = ulong.MaxValue
    }

    [Flags]
    [EnumSourceGen]
    public enum TestEnum : long
    {
        [Display(Name = "FirstDisplayName", Description = "FirstDescription")]
        First = 8,
        Second = 1,
        Third = 2,
        Other = 256,
        Min = long.MinValue
    }

    [EnumSourceGen]
    [EnumTransform(Preset = EnumTransform.UpperCase)]
    public enum TestTransformsEnum : long
    {
        [EnumTransformValue(ValueOverride = "ThisWasOverriden")]
        OverrideMe,
        uppercase
    }

    [EnumSourceGen]
    public enum TestOmitEnum
    {
        [EnumOmitValue]
        Omitted,
        [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
        OmittedWithFilter
    }
}

namespace Genbox.EnumSourceGen.Tests.Functionality.OtherNamespace
{
    [EnumSourceGen]
    public enum TestEnum
    {
        First,
        Second,
        Third
    }
}