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
        [EnumConfig(NameOverride = "ThisWasOverriden")]
        OverrideMe,
        [EnumConfig(SimpleTransform = EnumTransform.UpperCase)]
        UpperCase,
        [EnumConfig(SimpleTransform = EnumTransform.LowerCase)]
        LowerCase,
        [EnumConfig(AdvancedTransform = "ULULU____O")]
        MixedCase1,
        [EnumConfig(AdvancedTransform = "/1//")]
        R1e1m1o1v1e1A1l1l1O1n1e1s,
        [EnumConfig(Omit = true)]
        Omitted,
        [EnumConfig(Omit = true, OmitExclude = EnumOmitExclude.GetString)]
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