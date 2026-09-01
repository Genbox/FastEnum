using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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

    [FastEnum]
    public enum EscapedEnum
    {
        [EnumOmitValue]
        None = 0,
        [Display(Name = "C:\\Path\\File\"Name", Description = "Line1\\Line2")]
        [EnumTransformValue(ValueOverride = "Val\"With\\Slash")]
        Value1 = 42
    }

    [FastEnum(DisableCache = true)]
    internal enum UncachedEnum
    {
        First,
        Second
    }

    [FastEnum]
    [EnumTransform(SortMemberNames = EnumOrder.Ascending, SortMemberValues = EnumOrder.Descending, SortUnderlyingValues = EnumOrder.Ascending, SortDisplayNames = EnumOrder.Descending, SortDescriptions = EnumOrder.Ascending)]
    [SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "The fixture uses compact non-zero values to make every ordering observable.")]
    internal enum SortedEnum
    {
        [Display(Name = "Bravo", Description = "Second")]
        One = 2,
        [Display(Name = "Alpha", Description = "First")]
        Two = 1,
        [Display(Name = "Charlie", Description = "Third")]
        Three = 3
    }

    [FastEnum]
    internal enum OmitCoverageEnum
    {
        [Display(Name = "Keep display", Description = "Keep description")]
        Keep = 0,
        [EnumOmitValue]
        OmitAll = 1,
        [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberNames)]
        NoMemberName = 2,
        [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberValues)]
        NoMemberValue = 3,
        [EnumOmitValue(Exclude = EnumOmitExclude.GetUnderlyingValues)]
        NoUnderlyingValue = 4,
        [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
        NoUnderlyingLookup = 5,
        [EnumOmitValue(Exclude = EnumOmitExclude.TryParse)]
        NoParse = 6,
        [Display(Name = "Hidden display")]
        [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDisplayName)]
        NoDisplayLookup = 7,
        [Display(Description = "Hidden description")]
        [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDescription)]
        NoDescriptionLookup = 8,
        [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
        NoDefined = 9,
        [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
        NoString = 10,
        [EnumOmitValue(Exclude = EnumOmitExclude.None)]
        NoOmission = 11
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