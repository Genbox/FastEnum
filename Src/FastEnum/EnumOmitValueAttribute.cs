using System.Diagnostics;

namespace Genbox.FastEnum;

/// <summary>Controls which generated APIs omit an enum member.</summary>
[Conditional("FASTENUM_INCLUDE_ATTRIBUTE")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class EnumOmitValueAttribute : Attribute
{
    /// <summary>Choose which generated APIs should omit this enum member. Defaults to all when unspecified.</summary>
    public EnumOmitExclude Exclude { get; set; } = EnumOmitExclude.All;
}