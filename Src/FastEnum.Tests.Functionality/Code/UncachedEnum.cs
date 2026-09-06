namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum(DisableCache = true)]
internal enum UncachedEnum
{
    [global::System.ComponentModel.DataAnnotations.Display(Name = "First display", Description = "First description")]
    First,
    Second
}