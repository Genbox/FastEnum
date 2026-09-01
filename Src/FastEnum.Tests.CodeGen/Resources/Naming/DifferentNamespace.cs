// Same enum name in two different namespaces

namespace Some.Namespace.Here1
{
    [FastEnum]
    public enum MyEnum
    {
        Value
    }
}

namespace Some.Namespace.Here2
{
    [FastEnum]
    public enum MyEnum
    {
        Value
    }
}