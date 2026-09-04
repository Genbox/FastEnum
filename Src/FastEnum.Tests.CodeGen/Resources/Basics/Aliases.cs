// Aliases retain metadata and transformed names; lookup uses included numeric values.

[FastEnum]
[EnumTransform(Preset = EnumTransform.LowerCase)]
public enum Aliases
{
    None = 0,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    Omitted = 1,
    [Display(Name = "First", Description = "First description")]
    First = Omitted,
    [Display(Name = "Second", Description = "Second description")]
    Second = Omitted
}