using Genbox.EnumSourceGen.Extensions;
using Genbox.EnumSourceGen.Helpers;

namespace Genbox.EnumSourceGen.Data;

internal class EnumSourceGenData : IEquatable<EnumSourceGenData>
{
    public string? ExtensionClassName { get; set; }
    public string? ExtensionClassNamespace { get; set; }
    public string? EnumsClassName { get; set; }
    public string? EnumsClassNamespace { get; set; }
    public string? EnumNameOverride { get; set; }
    public bool DisableEnumsWrapper { get; set; }
    public bool DisableCache { get; set; }

    public bool Equals(EnumSourceGenData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ExtensionClassName == other.ExtensionClassName && ExtensionClassNamespace == other.ExtensionClassNamespace && EnumsClassName == other.EnumsClassName && EnumsClassNamespace == other.EnumsClassNamespace && EnumNameOverride == other.EnumNameOverride && DisableEnumsWrapper == other.DisableEnumsWrapper && DisableCache == other.DisableCache;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj.GetType() == GetType() && Equals((EnumSourceGenData)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (ExtensionClassName != null ? ExtensionClassName.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ExtensionClassNamespace != null ? ExtensionClassNamespace.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EnumsClassName != null ? EnumsClassName.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EnumsClassNamespace != null ? EnumsClassNamespace.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ (EnumNameOverride != null ? EnumNameOverride.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ DisableEnumsWrapper.GetHashCode();
            hashCode = (hashCode * 397) ^ DisableCache.GetHashCode();
            return hashCode;
        }
    }
}