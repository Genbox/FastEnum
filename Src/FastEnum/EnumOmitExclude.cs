namespace Genbox.FastEnum;

/// <summary>Specifies the generated APIs from which an enum member is omitted.</summary>
[Flags]
public enum EnumOmitExclude
{
    /// <summary>Do not omit the enum member from any generated API.</summary>
    None = 0,

    /// <summary>Omit the enum member from the generated member names.</summary>
    GetMemberNames = 1 << 0,

    /// <summary>Omit the enum member from the generated member values.</summary>
    GetMemberValues = 1 << 1,

    /// <summary>Omit the enum member from the generated underlying values.</summary>
    GetUnderlyingValues = 1 << 2,

    /// <summary>Omit the enum member from underlying-value lookup.</summary>
    TryGetUnderlyingValue = 1 << 3,

    /// <summary>Omit the enum member from parsing.</summary>
    TryParse = 1 << 4,

    /// <summary>Omit the enum member from display-name lookup.</summary>
    TryGetDisplayName = 1 << 5,

    /// <summary>Omit the enum member from description lookup.</summary>
    TryGetDescription = 1 << 6,

    /// <summary>Omit the enum member from defined-value checks.</summary>
    IsDefined = 1 << 7,

    /// <summary>Omit the enum member from string formatting.</summary>
    GetString = 1 << 8,

    /// <summary>Omit the enum member from all generated APIs.</summary>
    All = GetMemberNames | GetMemberValues | GetUnderlyingValues | TryGetUnderlyingValue | TryParse | TryGetDisplayName | TryGetDescription | IsDefined | GetString
}