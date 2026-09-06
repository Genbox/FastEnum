using Genbox.FastEnum.Extensions;

namespace Genbox.FastEnum.Data;

internal class EnumSpec : IEquatable<EnumSpec>
{
    //This class overrides equality to provide caching to the source generator framework. It cannot just be a record
    //as it contains lists that has to be compared, so we need to override equality anyway.

    public EnumSpec(string name, string emittedIdentifier, string fullName, string fullyQualifiedName, string? @namespace, Accessibility[] accessChain, bool hasGenericContainingType, bool hasDisplay, bool hasDescription, bool hasFlags, string underlyingType, FastEnumData data, EnumMemberSpec[] members, EnumTransformData? transformData, bool hasFileLocalType = false)
    {
        Name = name;
        EmittedIdentifier = emittedIdentifier;
        FullName = fullName;
        FullyQualifiedName = fullyQualifiedName;
        Namespace = @namespace;
        AccessChain = accessChain;
        HasGenericContainingType = hasGenericContainingType;
        HasFileLocalType = hasFileLocalType;
        HasDisplay = hasDisplay;
        HasDescription = hasDescription;
        HasFlags = hasFlags;
        UnderlyingType = underlyingType;
        Data = data;
        Members = members;
        TransformData = transformData;
    }

    public string Name { get; }
    public string EmittedIdentifier { get; }
    public string FullName { get; }
    public string FullyQualifiedName { get; }
    public string? Namespace { get; }
    public Accessibility[] AccessChain { get; }
    public bool IsPubliclyAccessible => Array.TrueForAll(AccessChain, x => x == Accessibility.Public);
    public bool HasGenericContainingType { get; }
    public bool HasFileLocalType { get; }
    public bool HasDisplay { get; }
    public bool HasDescription { get; }
    public bool HasFlags { get; }
    public string UnderlyingType { get; }
    public FastEnumData Data { get; }
    public EnumTransformData? TransformData { get; }
    public EnumMemberSpec[] Members { get; }

    public bool Equals(EnumSpec? other)
    {
        return other != null && Name == other.Name &&
               EmittedIdentifier == other.EmittedIdentifier &&
               FullName == other.FullName &&
               FullyQualifiedName == other.FullyQualifiedName &&
               Namespace == other.Namespace &&
               ListEqual(AccessChain, other.AccessChain) &&
               HasGenericContainingType == other.HasGenericContainingType &&
               HasFileLocalType == other.HasFileLocalType &&
               HasDisplay == other.HasDisplay &&
               HasDescription == other.HasDescription &&
               HasFlags == other.HasFlags &&
               UnderlyingType == other.UnderlyingType &&
               Data.Equals(other.Data) &&
               ListEqual(Members, other.Members) &&
               Equals(TransformData, other.TransformData);
    }

    public override bool Equals(object? obj) => obj is EnumSpec other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ EmittedIdentifier.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ FullName.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ FullyQualifiedName.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ (Namespace != null ? Namespace.GetDeterministicHashCode() : 0);
            hashCode = (hashCode * 397) ^ HasGenericContainingType.GetHashCode();
            hashCode = (hashCode * 397) ^ HasFileLocalType.GetHashCode();
            hashCode = (hashCode * 397) ^ HasDisplay.GetHashCode();
            hashCode = (hashCode * 397) ^ HasDescription.GetHashCode();
            hashCode = (hashCode * 397) ^ HasFlags.GetHashCode();
            hashCode = (hashCode * 397) ^ UnderlyingType.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ Data.GetHashCode();

            foreach (EnumMemberSpec member in Members)
            {
                hashCode = (hashCode * 397) ^ member.GetHashCode();
            }

            foreach (Accessibility ac in AccessChain)
            {
                hashCode = (hashCode * 397) ^ ac.GetHashCode();
            }

            hashCode = (hashCode * 397) ^ (TransformData != null ? TransformData.GetHashCode() : 0);
            return hashCode;
        }
    }

    private static bool ListEqual<T>(IList<T> first, IList<T> second)
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