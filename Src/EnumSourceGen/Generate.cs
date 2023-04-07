namespace Genbox.EnumSourceGen;

public enum Generate : byte
{
    /// <summary>Generate both class and extensions (default)</summary>
    ClassAndExtensions = 0,

    /// <summary>Only generate the Enums class.</summary>
    ClassOnly = 1,

    /// <summary>Only generate the enum extensions such as GetString(), GetUnderlyingValue(), etc.</summary>
    ExtensionsOnly = 2
}