using Genbox.FastEnum.Extensions;

namespace Genbox.FastEnum.Data;

internal class DisplayData : IEquatable<DisplayData>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public bool Equals(DisplayData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return Name == other.Name && Description == other.Description;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj.GetType() == GetType() && Equals((DisplayData)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Name != null ? Name.GetDeterministicHashCode() : 0) * 397) ^ (Description != null ? Description.GetDeterministicHashCode() : 0);
        }
    }
}