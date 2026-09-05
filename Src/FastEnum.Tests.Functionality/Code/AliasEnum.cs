using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

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