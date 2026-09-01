namespace Genbox.FastEnum;

/// <summary>Specifies the accessibility of a generated type.</summary>
public enum Visibility : byte
{
    /// <summary>Inherit accessibility from the annotated enum.</summary>
    Inherit,

    /// <summary>Generate an internal type.</summary>
    Internal,

    /// <summary>Generate a public type.</summary>
    Public
}