// Public and protected internal nested enums inherit internal helper visibility.

internal class InternalContainer
{
    public class Inner
    {
        [FastEnum]
        public enum Nested { None }
    }
}

public class PublicContainer
{
    [FastEnum]
    protected internal enum Restricted { None }
}