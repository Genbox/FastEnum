using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
public enum MetadataDispatchEnum
{
    None = 0,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryParse)]
    [Display(Name = "Shared", Description = "Details")]
    Omitted = 5,
    [EnumTransformValue(ValueOverride = "2")]
    [Display(Name = "Shared", Description = "Details")]
    First = 1,
    [Display(Name = "Shared", Description = "Details")]
    Second = 2,
    [EnumTransformValue(ValueOverride = "")]
    [Display(Name = "2", Description = "Shared")]
    Third = 3,
    [Display(Name = "SHARED", Description = "DETAILS")]
    Fourth = 4,
    [Display(Name = "Shadow", Description = "Detourx")]
    Fifth = 6
}