//ReSharper disable RedundantNameQualifier

namespace Genbox.FastEnum;

[global::System.Diagnostics.Conditional("FASTENUM_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumTransformAttribute : global::System.Attribute
{
    /// <summary>
    /// Transform all enum strings via a preset. Affects GetMemberNames() and GetString() output.
    /// </summary>
    public EnumTransform Preset { get; set; }

    /// <summary>
    /// Match a pattern and replace it with something else. For example: /regex-here/replacement-here/.
    /// Affects GetMemberNames() and GetString() output.
    /// </summary>
    public string? Regex { get; set; }

    /// <summary>
    /// Use a simple transform language string like 'UL__O_'. U = uppercase, L = lowercase, O = omit, _ = skip.
    /// Affects GetMemberNames() and GetString() output.
    /// </summary>
    public string? CasePattern { get; set; }

    /// <summary>Order applied to GetMemberNames() output.</summary>
    public EnumOrder SortMemberNames { get; set; } = EnumOrder.Ascending;

    /// <summary>Order applied to GetMemberValues() output.</summary>
    public EnumOrder SortMemberValues { get; set; } = EnumOrder.Ascending;

    /// <summary>Order applied to GetUnderlyingValues() output.</summary>
    public EnumOrder SortUnderlyingValues { get; set; } = EnumOrder.Ascending;

    /// <summary>Order applied to GetDisplayNames() output.</summary>
    public EnumOrder SortDisplayNames { get; set; } = EnumOrder.Ascending;

    /// <summary>Order applied to GetDescriptions() output.</summary>
    public EnumOrder SortDescriptions { get; set; } = EnumOrder.Ascending;
}
