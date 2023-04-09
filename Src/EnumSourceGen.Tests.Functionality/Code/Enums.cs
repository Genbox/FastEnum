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
    public enum TestTransformsEnum : long
    {
        [EnumTransformValue(NameOverride = "ThisWasOverriden")]
        OverrideMe,
        [EnumTransformValue(TransformPreset = EnumTransform.UpperCase)]
        UpperCase,
        [EnumTransformValue(TransformPreset = EnumTransform.LowerCase)]
        LowerCase,
        [EnumTransformValue(Transform = "ULULU____O")]
        MixedCase1,
        [EnumTransformValue(Transform = "/1//")]
        R1e1m1o1v1e1A1l1l1O1n1e1s,
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