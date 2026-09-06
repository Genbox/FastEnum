using System.Diagnostics;

namespace Genbox.FastEnum;

/// <summary>Overrides the generated string representation of an enum member.</summary>
[Conditional("FASTENUM_INCLUDE_ATTRIBUTE")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class EnumTransformValueAttribute : Attribute
{
    /// <summary>Override the generated string for this enum member. Affects GetString(), GetMemberNames(), and parsing.</summary>
    public string? ValueOverride { get; set; }
}