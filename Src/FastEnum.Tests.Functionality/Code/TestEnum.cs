using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[Flags]
[FastEnum]
public enum TestEnum : long
{
    [Display(Name = "FirstDisplayName", Description = "FirstDescription")]
    First = 8,
    Second = 1,
    Third = 2,
    Other = 256,
    Min = long.MinValue
}