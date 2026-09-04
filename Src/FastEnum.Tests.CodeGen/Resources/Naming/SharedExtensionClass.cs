// Multiple enums and user code can share the same partial extension class.

[FastEnum(ExtensionClassName = "SharedExtensions")]
public enum First { None }

[FastEnum(ExtensionClassName = "SharedExtensions")]
public enum Second { None }

public static partial class SharedExtensions
{
    public static string Custom() => "Custom";
}