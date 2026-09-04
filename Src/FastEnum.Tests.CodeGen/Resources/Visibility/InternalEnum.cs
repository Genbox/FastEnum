// A shared public wrapper preserves internal helpers/extensions for the internal enum.

namespace Some.Namespace.Here;

[FastEnum]
internal enum MyEnum
{
    First,
    Second,
    Third
}

[FastEnum]
public enum PublicEnum { None }