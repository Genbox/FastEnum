// Every C# enum underlying type should flow through flags generation

namespace Some.Namespace.Here;

[Flags]
[FastEnum]
public enum ByteEnum : byte
{
    None = 0,
    Value = byte.MaxValue
}

[Flags]
[FastEnum]
public enum SByteEnum : sbyte
{
    None = 0,
    Value = sbyte.MinValue
}

[Flags]
[FastEnum]
public enum ShortEnum : short
{
    None = 0,
    Value = short.MinValue
}

[Flags]
[FastEnum]
public enum UShortEnum : ushort
{
    None = 0,
    Value = ushort.MaxValue
}

[Flags]
[FastEnum]
public enum IntEnum : int
{
    None = 0,
    Value = int.MinValue
}

[Flags]
[FastEnum]
public enum UIntEnum : uint
{
    None = 0,
    Value = uint.MaxValue
}

[Flags]
[FastEnum]
public enum LongEnum : long
{
    None = 0,
    Value = long.MinValue
}

[Flags]
[FastEnum]
public enum ULongEnum : ulong
{
    None = 0,
    Value = ulong.MaxValue
}