// ReSharper disable RedundantNameQualifier

namespace Genbox.EnumSourceGen;

[global::System.Diagnostics.Conditional("ENUMSOURCEGEN_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
public sealed class EnumConfigAttribute : global::System.Attribute
{
    /// <summary>Set this to omit the value completely</summary>
    public bool Omit { get; set; }

    public EnumOmitExclude OmitExclude { get; set; }

    /// <summary>You cna set a completely different name with this property</summary>
    public string? NameOverride { get; set; }

    /// <summary>Use this to set the mode of transformation</summary>
    public EnumTransform SimpleTransform { get; set; }

    /// <summary>A string that defines a transform</summary>
    public string? AdvancedTransform { get; set; }
}