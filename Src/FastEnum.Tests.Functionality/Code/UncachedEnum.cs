using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum(DisableCache = true)]
internal enum UncachedEnum
{
    [Display(Name = "First display", Description = "First description")]
    First,
    Second
}