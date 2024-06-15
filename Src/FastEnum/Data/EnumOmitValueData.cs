namespace Genbox.FastEnum.Data;

internal class EnumOmitValueData : IEquatable<EnumOmitValueData>
{
    public EnumOmitExclude Exclude { get; set; }

    public bool Equals(EnumOmitValueData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return Exclude == other.Exclude;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj.GetType() == GetType() && Equals((EnumOmitValueData)obj);
    }

    public override int GetHashCode() => (int)Exclude;
}