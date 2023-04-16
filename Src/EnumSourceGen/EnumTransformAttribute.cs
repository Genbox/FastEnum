//ReSharper disable RedundantNameQualifier

namespace Genbox.EnumSourceGen;

[global::System.Diagnostics.Conditional("ENUMSOURCEGEN_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumTransformAttribute : global::System.Attribute
{
    /// <summary>Transform all enum strings on the enum to either </summary>
    public EnumTransform Preset { get; set; }

    /// <summary>Match a pattern and replace it with something else. For example: /regex-here/replacement-here/</summary>
    public string? Regex { get; set; }

    /// <summary>Use a simple transform language string like 'UL__O_'. U = uppercase, L = lowercase, O = omit, _ = skip</summary>
    public string? CaseSpec { get; set; }

    public EnumOrder SortMemberNames { get; set; }

    public EnumOrder SortMemberValues { get; set; }

    public EnumOrder SortUnderlyingValues { get; set; }

    public EnumOrder SortDisplayNames { get; set; }

    public EnumOrder SortDescriptions { get; set; }
}