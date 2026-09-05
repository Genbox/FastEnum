using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

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