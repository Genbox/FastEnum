using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
internal enum DisplayMetadataEnum
{
    [Display(Name = "First\u0085Second\u2028Third\u2029Fourth", Description = "Details\u2029Next", Order = 1, ShortName = "Short", GroupName = "Group", Prompt = "Prompt", AutoGenerateField = false, AutoGenerateFilter = true)]
    None
}