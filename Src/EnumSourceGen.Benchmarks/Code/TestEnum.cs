using System.ComponentModel.DataAnnotations;

namespace Genbox.EnumSourceGen.Benchmarks.Code;

[Flags]
[EnumSourceGen]
public enum TestEnum
{
    First = 0,

    [Display(Name = "2nd")]
    Second = 1,
    Third = 2
}