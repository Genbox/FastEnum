using System.ComponentModel.DataAnnotations;

namespace Genbox.EnumSourceGen.Benchmarks.Code;

[EnumSourceGen]
public enum TestEnum
{
    First,
    [Display(Name = "2nd")]
    Second,
    Third
}