//ReSharper disable RedundantNameQualifier

namespace Genbox.EnumSourceGen;

[global::System.Diagnostics.Conditional("ENUMSOURCEGEN_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Field)]
public sealed class EnumTransformValueAttribute : global::System.Attribute
{
    /// <summary>You cna set a completely different name with this property</summary>
    public string? ValueOverride { get; set; }
}