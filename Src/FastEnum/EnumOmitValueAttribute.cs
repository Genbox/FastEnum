// ReSharper disable RedundantNameQualifier

namespace Genbox.FastEnum;

[global::System.Diagnostics.Conditional("FASTENUM_INCLUDE_ATTRIBUTE")]
[global::System.AttributeUsage(AttributeTargets.Field)]
public sealed class EnumOmitValueAttribute : global::System.Attribute
{
    public EnumOmitExclude Exclude { get; set; }
}