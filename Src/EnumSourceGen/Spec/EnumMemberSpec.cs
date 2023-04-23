using Genbox.EnumSourceGen.Data;
using Genbox.EnumSourceGen.Extensions;
using Genbox.EnumSourceGen.Helpers;

namespace Genbox.EnumSourceGen.Spec;

internal class EnumMemberSpec : IEquatable<EnumMemberSpec>
{
    public EnumMemberSpec(string name, object value, DisplayData? displayData, EnumOmitValueData? omitValueData, EnumTransformValueData? transformValueData)
    {
        Name = name;
        Value = value;
        DisplayData = displayData;
        OmitValueData = omitValueData;
        TransformValueData = transformValueData;
    }

    public string Name { get; }
    public object Value { get; }
    public DisplayData? DisplayData { get; }
    public EnumOmitValueData? OmitValueData { get; }
    public EnumTransformValueData? TransformValueData { get; }

    public bool Equals(EnumMemberSpec? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name &&
               Value.Equals(other.Value) &&
               Equals(DisplayData, other.DisplayData) &&
               Equals(OmitValueData, other.OmitValueData) &&
               Equals(TransformValueData, other.TransformValueData);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EnumMemberSpec)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Name.GetDeterministicHashCode();
            hashCode = (hashCode * 397) ^ Value.GetHashCode();
            hashCode = (hashCode * 397) ^ (DisplayData != null ? DisplayData.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (OmitValueData != null ? OmitValueData.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (TransformValueData != null ? TransformValueData.GetHashCode() : 0);
            return hashCode;
        }
    }
}