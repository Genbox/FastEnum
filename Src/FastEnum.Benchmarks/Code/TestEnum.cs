using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Benchmarks.Code;

[FastEnum]
public enum TestEnum
{
    First,
    [Display(Name = "2nd")]
    Second,
    Third
}