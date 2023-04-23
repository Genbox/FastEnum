using Genbox.EnumSourceGen.Data;
using Genbox.EnumSourceGen.Extensions;
using Genbox.EnumSourceGen.Helpers;

namespace Genbox.EnumSourceGen.Spec;

internal class EnumSpec : IEquatable<EnumSpec>
{
    public EnumSpec(string name, string fullName, string fullyQualifiedName, string? @namespace, bool isPublic, bool hasDisplay, bool hasDescription, bool hasFlags, string underlyingType, EnumSourceGenData sourceGenData, List<EnumMemberSpec> members, EnumTransformData? transformData)
    {
        Name = name;
        FullName = fullName;
        FullyQualifiedName = fullyQualifiedName;
        Namespace = @namespace;
        IsPublic = isPublic;
        HasDisplay = hasDisplay;
        HasDescription = hasDescription;
        HasFlags = hasFlags;
        UnderlyingType = underlyingType;
        SourceGenData = sourceGenData;
        Members = members;
        TransformData = transformData;
    }

    public string Name { get; }
    public string FullName { get; }
    public string FullyQualifiedName { get; }
    public string? Namespace { get; }
    public bool IsPublic { get; }
    public bool HasDisplay { get; }
    public bool HasDescription { get; }
    public bool HasFlags { get; }
    public string UnderlyingType { get; }
    public EnumSourceGenData SourceGenData { get; }
    public EnumTransformData? TransformData { get; }
    public List<EnumMemberSpec> Members { get; }

    public bool Equals(EnumSpec other)
    {
        return Name == other.Name &&
               FullName == other.FullName &&
               FullyQualifiedName == other.FullyQualifiedName &&
               Namespace == other.Namespace &&
               IsPublic == other.IsPublic &&
               HasDisplay == other.HasDisplay &&
               HasDescription == other.HasDescription &&
               HasFlags == other.HasFlags &&
               UnderlyingType == other.UnderlyingType &&
               SourceGenData.Equals(other.SourceGenData) &&
               MembersEqual(other.Members) &&
               Equals(TransformData, other.TransformData);
    }

    private bool MembersEqual(List<EnumMemberSpec> other)
    {
        if (Members.Count != other.Count)
            return false;

        for (int i = 0; i < other.Count; i++)
        {
            if (!Equals(Members[i], other[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        return Equals((EnumSpec)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ FullName.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ FullyQualifiedName.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsPublic.GetHashCode();
            hashCode = (hashCode * 397) ^ HasDisplay.GetHashCode();
            hashCode = (hashCode * 397) ^ HasDescription.GetHashCode();
            hashCode = (hashCode * 397) ^ HasFlags.GetHashCode();
            hashCode = (hashCode * 397) ^ UnderlyingType.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ SourceGenData.GetHashCode();

            foreach (EnumMemberSpec member in Members)
            {
                hashCode = (hashCode * 397) ^ member.GetHashCode();
            }

            hashCode = (hashCode * 397) ^ (TransformData != null ? TransformData.GetHashCode() : 0);
            return hashCode;
        }
    }
}