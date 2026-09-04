using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
[EnumTransform(Preset = EnumTransform.LowerCase)]
internal enum TransformedAliasEnum
{
    None = 0,
    First = 1,
    Alias = First
}

[FastEnum]
internal enum AliasEnum
{
    None = 0,
    [Display(Name = "First name", Description = "First description")]
    First = 1,
    [Display(Name = "Alias name", Description = "Alias description")]
    Alias = First,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    OmittedAlias = First,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDisplayName | EnumOmitExclude.TryGetDescription)]
    [Display(Name = "Omitted name", Description = "Omitted description")]
    OmittedMetadata = 2,
    [Display(Name = "Included name", Description = "Included description")]
    IncludedMetadata = OmittedMetadata,
    MissingMetadata = 3,
    [Display(Name = "Available name", Description = "Available description")]
    AvailableMetadata = MissingMetadata
}

[FastEnum]
internal enum OmittedFormattingEnum
{
    None = 0,
    [EnumOmitValue]
    [Display(Name = "Hidden name", Description = "Hidden description")]
    OmitAll = 1,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
    [Display(Name = "Retained name", Description = "Retained description")]
    OmitString = 2,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
    WithoutMetadata = 3
}