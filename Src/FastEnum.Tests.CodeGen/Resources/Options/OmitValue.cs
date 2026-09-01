// Omit specific members per API

[FastEnum]
internal enum MyEnum
{
    Keep = 0,

    [EnumOmitValue]
    OmitAll,

    [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberNames | EnumOmitExclude.TryParse)]
    OmitNameParse,

    [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberValues)]
    OmitMemberValue,

    [EnumOmitValue(Exclude = EnumOmitExclude.GetUnderlyingValues)]
    OmitUnderlying,

    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
    OmitUnderlyingLookup,

    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDisplayName | EnumOmitExclude.TryGetDescription)]
    [Display(Name = "Disp", Description = "Desc")]
    OmitDisplay,

    [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
    OmitIsDefined,

    [EnumOmitValue(Exclude = EnumOmitExclude.GetString)]
    OmitString,

    [EnumOmitValue(Exclude = EnumOmitExclude.None)]
    KeepExplicitly,
}