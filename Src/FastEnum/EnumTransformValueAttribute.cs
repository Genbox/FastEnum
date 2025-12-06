//ReSharper disable RedundantNameQualifier

namespace Genbox.FastEnum;

[global::System.Diagnostics.Conditional("FASTENUM_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Field)]
public sealed class EnumTransformValueAttribute : global::System.Attribute
{
    /// <summary>Override the generated string for this enum member. Affects GetString(), GetMemberNames(), and parsing.</summary>
    public string? ValueOverride { get; set; }
}