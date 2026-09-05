using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
internal enum OmitCoverageEnum
{
    [Display(Name = "Keep display", Description = "Keep description")]
    Keep = 0,
    [EnumOmitValue]
    OmitAll = 1,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberNames)]
    NoMemberName = 2,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberValues)]
    NoMemberValue = 3,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetUnderlyingValues)]
    NoUnderlyingValue = 4,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    NoUnderlyingLookup = 5,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryParse)]
    NoParse = 6,
    [Display(Name = "Hidden display")]
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDisplayName)]
    NoDisplayLookup = 7,
    [Display(Description = "Hidden description")]
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDescription)]
    NoDescriptionLookup = 8,
    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
    NoDefined = 9,
    [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
    NoString = 10,
    [EnumOmitValue(Exclude = EnumOmitExclude.None)]
    NoOmission = 11
}