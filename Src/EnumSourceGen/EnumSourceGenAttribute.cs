// ReSharper disable RedundantNameQualifier
namespace Genbox.EnumSourceGen;

/// <summary>Add to enums to indicate that extension methods should be generated for the type</summary>
[global::System.Diagnostics.Conditional("ENUMSOURCEGEN_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumSourceGenAttribute : global::System.Attribute
{
    /// <summary>
    /// Override the name of the extension class. If not provided, the enum name with "Extensions" appended will be used. For example for an Enum called StatusCodes, the default name
    /// will be StatusCodesExtensions
    /// </summary>
    public string? ExtensionClassName { get; set; }

    /// <summary>The namespace used for the extensions class. If not provided the namespace of the enum will be used</summary>
    public string? ExtensionClassNamespace { get; set; }

    /// <summary>Override the name of the Enums class. If not provided, it will be "Enums"</summary>
    public string? EnumsClassName { get; set; }

    /// <summary>The namespace use for Enums class. If not provided the namespace of the enum will be used</summary>
    public string? EnumsClassNamespace { get; set; }

    /// <summary>Use this to override the name of your enum. This is useful in cases where you have overridden the namespace for the generated code, but now have two enums named the same</summary>
    public string? EnumNameOverride { get; set; }

    /// <summary>Enable this to avoid generating the static Enums class that wraps all the enums. Enums.MyEnum becomes MyEnum. This is handy if you want to set all enums inside the same namespace across projects</summary>
    public bool DisableEnumsWrapper { get; set; }

    /// <summary>By default arrays from GetValues(), GetNames() etc. is cached. Set this to true to avoid caching.</summary>
    public bool DisableCache { get; set; }
}