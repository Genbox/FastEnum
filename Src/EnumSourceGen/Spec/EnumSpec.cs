using Genbox.EnumSourceGen.Data;
using Genbox.EnumSourceGen.Extensions;
using Microsoft.CodeAnalysis;

namespace Genbox.EnumSourceGen.Spec;

internal class EnumSpec : IEquatable<EnumSpec>
{
    public EnumSpec(string name, string fullName, string fullyQualifiedName, string? @namespace, Accessibility[] accessChain, bool hasDisplay, bool hasDescription, bool hasFlags, string underlyingType, EnumSourceGenData sourceGenData, EnumMemberSpec[] members, EnumTransformData? transformData)
    {
        Name = name;
        FullName = fullName;
        FullyQualifiedName = fullyQualifiedName;
        Namespace = @namespace;
        AccessChain = accessChain;
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
    public Accessibility[] AccessChain { get; }
    public bool HasDisplay { get; }
    public bool HasDescription { get; }
    public bool HasFlags { get; }
    public string UnderlyingType { get; }
    public EnumSourceGenData SourceGenData { get; }
    public EnumTransformData? TransformData { get; }
    public EnumMemberSpec[] Members { get; }

    public bool Equals(EnumSpec other)
    {
        return Name == other.Name &&
               FullName == other.FullName &&
               FullyQualifiedName == other.FullyQualifiedName &&
               Namespace == other.Namespace &&
               ListEqual(AccessChain, other.AccessChain) &&
               HasDisplay == other.HasDisplay &&
               HasDescription == other.HasDescription &&
               HasFlags == other.HasFlags &&
               UnderlyingType == other.UnderlyingType &&
               SourceGenData.Equals(other.SourceGenData) &&
               ListEqual(Members, other.Members) &&
               Equals(TransformData, other.TransformData);
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
            hashCode = (hashCode * 397) ^ AccessChain.GetHashCode();
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

    private bool ListEqual<T>(IList<T> first, IList<T> second)
    {
        if (first.Count != second.Count)
            return false;

        for (int i = 0; i < first.Count; i++)
        {
            if (!Equals(first[i], second[i]))
                return false;
        }

        return true;
    }
}