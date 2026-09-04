// Disable cache to force array recreation

namespace Some.Namespace.Here;

[FastEnum(DisableCache = true)]
public enum MyEnum
{
    [Display(Name = "Name", Description = "Description")]
    Value1,
}