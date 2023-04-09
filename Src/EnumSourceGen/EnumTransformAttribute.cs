//ReSharper disable RedundantNameQualifier

namespace Genbox.EnumSourceGen;

[global::System.Diagnostics.Conditional("ENUMSOURCEGEN_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumTransformAttribute : global::System.Attribute
{
    /// <summary>Transform all enum strings on the enum to either </summary>
    public EnumTransform Transform { get; set; }

    public EnumOrder SortMemberNames { get; set; }

    public EnumOrder SortMemberValues { get; set; }

    public EnumOrder SortUnderlyingValues { get; set; }

    public EnumOrder SortDisplayNames { get; set; }

    public EnumOrder SortDescriptions { get; set; }
}