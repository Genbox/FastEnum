// ReSharper disable RedundantNameQualifier
namespace Genbox.EnumSourceGen;

/// <summary>Add to enums to indicate that extension methods should be generated for the type</summary>
[global::System.AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumSourceGenAttribute : global::System.Attribute
{
    /// <summary>Use this to control if the static Enums class member and/or enum extension should be generated </summary>
    public Generate Generate { get; set; }

    /// <summary>The namespace used for the extensions class. If not provided the namespace of the enum will be used</summary>
    public string? ExtensionClassNamespace { get; set; }

    /// <summary>
    /// Override the name of the extension class. If not provided, the enum name with "Extensions" appended will be used. For example for an Enum called StatusCodes, the default name
    /// will be StatusCodesExtensions
    /// </summary>
    public string? ExtensionClassName { get; set; }

    /// <summary>The namespace use for Enums class. If not provided the namespace of the enum will be used</summary>
    public string? EnumsClassNamespace { get; set; }

    /// <summary>Override the name of the Enums class. If not provided, it will be "Enums"</summary>
    public string? EnumsClassName { get; set; }
}