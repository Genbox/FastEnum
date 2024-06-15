using Genbox.FastEnum.Extensions;

namespace Genbox.FastEnum.Data;

internal class EnumTransformData : IEquatable<EnumTransformData>
{
    public EnumTransform Preset { get; set; }
    public string? Regex { get; set; }
    public string? CasePattern { get; set; }
    public EnumOrder SortMemberNames { get; set; }
    public EnumOrder SortMemberValues { get; set; }
    public EnumOrder SortUnderlyingValues { get; set; }
    public EnumOrder SortDisplayNames { get; set; }
    public EnumOrder SortDescriptions { get; set; }

    public bool Equals(EnumTransformData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return Preset == other.Preset && Regex == other.Regex && CasePattern == other.CasePattern && SortMemberNames == other.SortMemberNames && SortMemberValues == other.SortMemberValues && SortUnderlyingValues == other.SortUnderlyingValues && SortDisplayNames == other.SortDisplayNames && SortDescriptions == other.SortDescriptions;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj.GetType() == GetType() && Equals((EnumTransformData)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (int)Preset;
            hashCode = (hashCode * 397) ^ (Regex != null ? Regex.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ (CasePattern != null ? CasePattern.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)SortMemberNames;
            hashCode = (hashCode * 397) ^ (int)SortMemberValues;
            hashCode = (hashCode * 397) ^ (int)SortUnderlyingValues;
            hashCode = (hashCode * 397) ^ (int)SortDisplayNames;
            hashCode = (hashCode * 397) ^ (int)SortDescriptions;
            return hashCode;
        }
    }
}