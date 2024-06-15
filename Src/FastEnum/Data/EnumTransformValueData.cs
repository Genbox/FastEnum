using Genbox.FastEnum.Extensions;

namespace Genbox.FastEnum.Data;

internal class EnumTransformValueData : IEquatable<EnumTransformValueData>
{
    public string ValueOverride { get; set; }

    public bool Equals(EnumTransformValueData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ValueOverride == other.ValueOverride;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj.GetType() == GetType() && Equals((EnumTransformValueData)obj);
    }

    public override int GetHashCode() => ValueOverride.GetDeterministicHashCode();
}