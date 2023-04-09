//ReSharper disable RedundantNameQualifier

namespace Genbox.EnumSourceGen;

[global::System.Diagnostics.Conditional("ENUMSOURCEGEN_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Field)]
public sealed class EnumTransformValueAttribute : global::System.Attribute
{
    /// <summary>You cna set a completely different name with this property</summary>
    public string? NameOverride { get; set; }

    /// <summary>A simple way to transform enum strings</summary>
    public EnumTransform TransformPreset { get; set; }

    /// <summary>Set either a transform language string like 'UL__O_' or a regex like /regex-here/replacement-here/</summary>
    public string? Transform { get; set; }
}