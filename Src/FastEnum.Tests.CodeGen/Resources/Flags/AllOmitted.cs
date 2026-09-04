// All-omitted and empty flags have no defined values.

[FastEnum]
[Flags]
public enum AllOmitted
{
    [EnumOmitValue]
    None = 0,
    [EnumOmitValue]
    First = 1
}

[FastEnum]
[Flags]
public enum EmptyFlags { }