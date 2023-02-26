using System.ComponentModel.DataAnnotations;

namespace Genbox.EnumSourceGen.Tests.Code
{
    [Flags]
    [EnumSourceGen]
    public enum TestEnum
    {
        [Display(Name = "FirstDisplayName", Description = "FirstDescription")]
        First = 8,
        Second = 1,
        Third = 2,
        Other = 256
    }
}

namespace Genbox.EnumSourceGen.Tests.Code2
{
    [EnumSourceGen]
    public enum TestEnum
    {
        First,
        Second,
        Third
    }
}