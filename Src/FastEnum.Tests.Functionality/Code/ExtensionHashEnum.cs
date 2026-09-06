using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
internal enum ExtensionHashEnum : long
{
    [EnumOmitValue(Exclude = EnumOmitExclude.TryGetDisplayName | EnumOmitExclude.TryGetDescription | EnumOmitExclude.TryGetUnderlyingValue)]
    [Display(Name = "Excluded alias", Description = "Excluded alias")]
    Alias = 0,
    [Display(Name = "Label0", Description = "Detail0")]
    Value0 = Alias,
    [EnumOmitValue(Exclude = EnumOmitExclude.All)]
    [Display(Name = "Label1", Description = "Detail1")]
    Value1 = 256,
    Value2 = 512,
    [Display(Name = "Label3", Description = "Detail3")]
    Value3 = 768,
    [Display(Name = "Label4", Description = "Detail4")]
    Value4 = 1024,
    [Display(Name = "Label5", Description = "Detail5")]
    Value5 = 1280,
    [Display(Name = "Label6", Description = "Detail6")]
    Value6 = 1536,
    [Display(Name = "Label7", Description = "Detail7")]
    Value7 = 1792,
    [Display(Name = "Label8", Description = "Detail8")]
    Value8 = 2048,
    [Display(Name = "Label9", Description = "Detail9")]
    Value9 = 2304,
    [Display(Name = "Label10", Description = "Detail10")]
    Value10 = 2560,
    [Display(Name = "Label11", Description = "Detail11")]
    Value11 = 2816,
    [Display(Name = "Label12", Description = "Detail12")]
    Value12 = 3072,
    [Display(Name = "Label13", Description = "Detail13")]
    Value13 = 3328,
    [Display(Name = "Label14", Description = "Detail14")]
    Value14 = 3584,
    [Display(Name = "Label15", Description = "Detail15")]
    Value15 = 3840,
    [Display(Name = "Label16", Description = "Detail16")]
    Value16 = 4096,
    [Display(Name = "Label17", Description = "Detail17")]
    Value17 = 4352,
    [Display(Name = "Label18", Description = "Detail18")]
    Value18 = 4608,
    [Display(Name = "Label19", Description = "Detail19")]
    Value19 = 4864,
    [Display(Name = "Label20", Description = "Detail20")]
    Value20 = 5120,
    [Display(Name = "Label21", Description = "Detail21")]
    Value21 = 5376,
    [Display(Name = "Label22", Description = "Detail22")]
    Value22 = 5632,
    [Display(Name = "Label23", Description = "Detail23")]
    Value23 = 5888,
    [Display(Name = "Label24", Description = "Detail24")]
    Value24 = 6144,
    [Display(Name = "Label25", Description = "Detail25")]
    Value25 = 6400,
    [Display(Name = "Label26", Description = "Detail26")]
    Value26 = 6656,
    [Display(Name = "Label27", Description = "Detail27")]
    Value27 = 6912,
    [Display(Name = "Label28", Description = "Detail28")]
    Value28 = 7168,
    [Display(Name = "Label29", Description = "Detail29")]
    Value29 = 7424,
    [Display(Name = "Label30", Description = "Detail30")]
    Value30 = 7680,
    [Display(Name = "Label31", Description = "Detail31")]
    Value31 = 7936,
    [Display(Name = "Label32", Description = "Detail32")]
    Value32 = 8192,
    [Display(Name = "Label33", Description = "Detail33")]
    Value33 = 8448,
    [Display(Name = "Label34", Description = "Detail34")]
    Value34 = 8704,
    [Display(Name = "Label35", Description = "Detail35")]
    Value35 = 8960,
    [Display(Name = "Label36", Description = "Detail36")]
    Value36 = 9216,
    [Display(Name = "Label37", Description = "Detail37")]
    Value37 = 9472,
    [Display(Name = "Label38", Description = "Detail38")]
    Value38 = 9728,
    [Display(Name = "Label39", Description = "Detail39")]
    Value39 = 9984,
}