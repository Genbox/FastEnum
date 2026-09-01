namespace Genbox.FastEnum;

/// <summary>Specifies a predefined transformation for generated enum names.</summary>
public enum EnumTransform
{
    /// <summary>Do not transform enum names.</summary>
    None = 0,

    /// <summary>Convert enum names to lowercase.</summary>
    LowerCase,

    /// <summary>Convert enum names to uppercase.</summary>
    UpperCase
}