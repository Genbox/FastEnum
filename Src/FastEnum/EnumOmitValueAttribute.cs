// ReSharper disable RedundantNameQualifier

namespace Genbox.FastEnum;

[global::System.Diagnostics.Conditional("FASTENUM_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Field)]
public sealed class EnumOmitValueAttribute : global::System.Attribute
{
    /// <summary>Choose which generated APIs should omit this enum member. Defaults to all when unspecified.</summary>
    public EnumOmitExclude Exclude { get; set; } = EnumOmitExclude.All;
}