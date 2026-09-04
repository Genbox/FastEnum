using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
public enum LargeLookupEnum : long
{
    Min = long.MinValue,
    Max = long.MaxValue,
    Negative = -5,
    Alias = 18,
    [Display(Name = "Label159", Description = "Detail159")]
    Value159 = 477,
    [Display(Name = "Label158", Description = "Detail158")]
    Value158 = 474,
    [Display(Name = "Label157", Description = "Detail157")]
    Value157 = 471,
    [Display(Name = "Label156", Description = "Detail156")]
    Value156 = 468,
    [Display(Name = "Label155", Description = "Detail155")]
    Value155 = 465,
    [Display(Name = "Label154", Description = "Detail154")]
    Value154 = 462,
    [Display(Name = "Label153", Description = "Detail153")]
    Value153 = 459,
    [Display(Name = "Label152", Description = "Detail152")]
    Value152 = 456,
    [Display(Name = "Label151", Description = "Detail151")]
    Value151 = 453,
    [Display(Name = "Label150", Description = "Detail150")]
    Value150 = 450,
    [Display(Name = "Label149", Description = "Detail149")]
    Value149 = 447,
    [Display(Name = "Label148", Description = "Detail148")]
    Value148 = 444,
    [Display(Name = "Label147", Description = "Detail147")]
    Value147 = 441,
    [Display(Name = "Label146", Description = "Detail146")]
    Value146 = 438,
    [Display(Name = "Label145", Description = "Detail145")]
    Value145 = 435,
    [Display(Name = "Label144", Description = "Detail144")]
    Value144 = 432,
    [Display(Name = "Label143", Description = "Detail143")]
    Value143 = 429,
    [Display(Name = "Label142", Description = "Detail142")]
    Value142 = 426,
    [Display(Name = "Label141", Description = "Detail141")]
    Value141 = 423,
    [Display(Name = "Label140", Description = "Detail140")]
    Value140 = 420,
    [Display(Name = "Label139", Description = "Detail139")]
    Value139 = 417,
    [Display(Name = "Label138", Description = "Detail138")]
    Value138 = 414,
    [Display(Name = "Label137", Description = "Detail137")]
    Value137 = 411,
    [Display(Name = "Label136", Description = "Detail136")]
    Value136 = 408,
    [Display(Name = "Label135", Description = "Detail135")]
    Value135 = 405,
    [Display(Name = "Label134", Description = "Detail134")]
    Value134 = 402,
    [Display(Name = "Label133", Description = "Detail133")]
    Value133 = 399,
    [Display(Name = "Label132", Description = "Detail132")]
    Value132 = 396,
    [Display(Name = "Label131", Description = "Detail131")]
    Value131 = 393,
    [Display(Name = "Label130", Description = "Detail130")]
    Value130 = 390,
    [Display(Name = "Label129", Description = "Detail129")]
    Value129 = 387,
    [Display(Name = "Label128", Description = "Detail128")]
    Value128 = 384,
    [Display(Name = "Label127", Description = "Detail127")]
    Value127 = 381,
    [Display(Name = "Label126", Description = "Detail126")]
    Value126 = 378,
    [Display(Name = "Label125", Description = "Detail125")]
    Value125 = 375,
    [Display(Name = "Label124", Description = "Detail124")]
    Value124 = 372,
    [Display(Name = "Label123", Description = "Detail123")]
    Value123 = 369,
    [Display(Name = "Label122", Description = "Detail122")]
    Value122 = 366,
    [Display(Name = "Label121", Description = "Detail121")]
    Value121 = 363,
    [Display(Name = "Label120", Description = "Detail120")]
    Value120 = 360,
    [Display(Name = "Label119", Description = "Detail119")]
    Value119 = 357,
    [Display(Name = "Label118", Description = "Detail118")]
    Value118 = 354,
    [Display(Name = "Label117", Description = "Detail117")]
    Value117 = 351,
    [Display(Name = "Label116", Description = "Detail116")]
    Value116 = 348,
    [Display(Name = "Label115", Description = "Detail115")]
    Value115 = 345,
    [Display(Name = "Label114", Description = "Detail114")]
    Value114 = 342,
    [Display(Name = "Label113", Description = "Detail113")]
    Value113 = 339,
    [Display(Name = "Label112", Description = "Detail112")]
    Value112 = 336,
    [Display(Name = "Label111", Description = "Detail111")]
    Value111 = 333,
    [Display(Name = "Label110", Description = "Detail110")]
    Value110 = 330,
    [Display(Name = "Label109", Description = "Detail109")]
    Value109 = 327,
    [Display(Name = "Label108", Description = "Detail108")]
    Value108 = 324,
    [Display(Name = "Label107", Description = "Detail107")]
    Value107 = 321,
    [Display(Name = "Label106", Description = "Detail106")]
    Value106 = 318,
    [Display(Name = "Label105", Description = "Detail105")]
    Value105 = 315,
    [Display(Name = "Label104", Description = "Detail104")]
    Value104 = 312,
    [Display(Name = "Label103", Description = "Detail103")]
    Value103 = 309,
    [Display(Name = "Label102", Description = "Detail102")]
    Value102 = 306,
    [Display(Name = "Label101", Description = "Detail101")]
    Value101 = 303,
    [Display(Name = "Label100", Description = "Detail100")]
    Value100 = 300,
    [Display(Name = "Label99", Description = "Detail99")]
    Value99 = 297,
    [Display(Name = "Label98", Description = "Detail98")]
    Value98 = 294,
    [Display(Name = "Label97", Description = "Detail97")]
    Value97 = 291,
    [Display(Name = "Label96", Description = "Detail96")]
    Value96 = 288,
    [Display(Name = "Label95", Description = "Detail95")]
    Value95 = 285,
    [Display(Name = "Label94", Description = "Detail94")]
    Value94 = 282,
    [Display(Name = "Label93", Description = "Detail93")]
    Value93 = 279,
    [Display(Name = "Label92", Description = "Detail92")]
    Value92 = 276,
    [Display(Name = "Label91", Description = "Detail91")]
    Value91 = 273,
    [Display(Name = "Label90", Description = "Detail90")]
    Value90 = 270,
    [Display(Name = "Label89", Description = "Detail89")]
    Value89 = 267,
    [Display(Name = "Label88", Description = "Detail88")]
    Value88 = 264,
    [Display(Name = "Label87", Description = "Detail87")]
    Value87 = 261,
    [Display(Name = "Label86", Description = "Detail86")]
    Value86 = 258,
    [Display(Name = "Label85", Description = "Detail85")]
    Value85 = 255,
    [Display(Name = "Label84", Description = "Detail84")]
    Value84 = 252,
    [Display(Name = "Label83", Description = "Detail83")]
    Value83 = 249,
    [Display(Name = "Label82", Description = "Detail82")]
    Value82 = 246,
    [Display(Name = "Label81", Description = "Detail81")]
    Value81 = 243,
    [Display(Name = "Label80", Description = "Detail80")]
    Value80 = 240,
    [Display(Name = "Label79", Description = "Detail79")]
    Value79 = 237,
    [Display(Name = "Label78", Description = "Detail78")]
    Value78 = 234,
    [Display(Name = "Label77", Description = "Detail77")]
    Value77 = 231,
    [Display(Name = "Label76", Description = "Detail76")]
    Value76 = 228,
    [Display(Name = "Label75", Description = "Detail75")]
    Value75 = 225,
    [Display(Name = "Label74", Description = "Detail74")]
    Value74 = 222,
    [Display(Name = "Label73", Description = "Detail73")]
    Value73 = 219,
    [Display(Name = "Label72", Description = "Detail72")]
    Value72 = 216,
    [Display(Name = "Label71", Description = "Detail71")]
    Value71 = 213,
    [Display(Name = "Label70", Description = "Detail70")]
    Value70 = 210,
    [Display(Name = "Label69", Description = "Detail69")]
    Value69 = 207,
    [Display(Name = "Label68", Description = "Detail68")]
    Value68 = 204,
    [Display(Name = "Label67", Description = "Detail67")]
    Value67 = 201,
    [Display(Name = "Label66", Description = "Detail66")]
    Value66 = 198,
    [Display(Name = "Label65", Description = "Detail65")]
    Value65 = 195,
    [Display(Name = "Label64", Description = "Detail64")]
    Value64 = 192,
    [Display(Name = "Label63", Description = "Detail63")]
    Value63 = 189,
    [Display(Name = "Label62", Description = "Detail62")]
    Value62 = 186,
    [Display(Name = "Label61", Description = "Detail61")]
    Value61 = 183,
    [Display(Name = "Label60", Description = "Detail60")]
    Value60 = 180,
    [Display(Name = "Label59", Description = "Detail59")]
    Value59 = 177,
    [Display(Name = "Label58", Description = "Detail58")]
    Value58 = 174,
    [Display(Name = "Label57", Description = "Detail57")]
    Value57 = 171,
    [Display(Name = "Label56", Description = "Detail56")]
    Value56 = 168,
    [Display(Name = "Label55", Description = "Detail55")]
    Value55 = 165,
    [Display(Name = "Label54", Description = "Detail54")]
    Value54 = 162,
    [Display(Name = "Label53", Description = "Detail53")]
    Value53 = 159,
    [Display(Name = "Label52", Description = "Detail52")]
    Value52 = 156,
    [Display(Name = "Label51", Description = "Detail51")]
    Value51 = 153,
    [Display(Name = "Label50", Description = "Detail50")]
    Value50 = 150,
    [Display(Name = "Label49", Description = "Detail49")]
    Value49 = 147,
    [Display(Name = "Label48", Description = "Detail48")]
    Value48 = 144,
    [Display(Name = "Label47", Description = "Detail47")]
    Value47 = 141,
    [Display(Name = "Label46", Description = "Detail46")]
    Value46 = 138,
    [Display(Name = "Label45", Description = "Detail45")]
    Value45 = 135,
    [Display(Name = "Label44", Description = "Detail44")]
    Value44 = 132,
    [Display(Name = "Label43", Description = "Detail43")]
    Value43 = 129,
    [Display(Name = "Label42", Description = "Detail42")]
    Value42 = 126,
    [Display(Name = "Label41", Description = "Detail41")]
    Value41 = 123,
    [Display(Name = "Label40", Description = "Detail40")]
    Value40 = 120,
    [Display(Name = "Label39", Description = "Detail39")]
    Value39 = 117,
    [Display(Name = "Label38", Description = "Detail38")]
    Value38 = 114,
    [Display(Name = "Label37", Description = "Detail37")]
    Value37 = 111,
    [Display(Name = "Label36", Description = "Detail36")]
    Value36 = 108,
    [Display(Name = "Label35", Description = "Detail35")]
    Value35 = 105,
    [Display(Name = "Label34", Description = "Detail34")]
    Value34 = 102,
    [Display(Name = "Label33", Description = "Detail33")]
    Value33 = 99,
    [Display(Name = "Label32", Description = "Detail32")]
    Value32 = 96,
    [Display(Name = "Label31", Description = "Detail31")]
    Value31 = 93,
    [Display(Name = "Label30", Description = "Detail30")]
    Value30 = 90,
    [Display(Name = "Label29", Description = "Detail29")]
    Value29 = 87,
    [Display(Name = "Label28", Description = "Detail28")]
    Value28 = 84,
    [Display(Name = "Label27", Description = "Detail27")]
    Value27 = 81,
    [Display(Name = "Label26", Description = "Detail26")]
    Value26 = 78,
    [Display(Name = "Label25", Description = "Detail25")]
    Value25 = 75,
    [Display(Name = "Label24", Description = "Detail24")]
    Value24 = 72,
    [Display(Name = "Label23", Description = "Detail23")]
    Value23 = 69,
    [Display(Name = "Label22", Description = "Detail22")]
    Value22 = 66,
    [Display(Name = "Label21", Description = "Detail21")]
    Value21 = 63,
    [Display(Name = "Label20", Description = "Detail20")]
    Value20 = 60,
    [Display(Name = "Label19", Description = "Detail19")]
    Value19 = 57,
    [Display(Name = "Label18", Description = "Detail18")]
    Value18 = 54,
    [Display(Name = "Label17", Description = "Detail17")]
    Value17 = 51,
    [Display(Name = "Label16", Description = "Detail16")]
    Value16 = 48,
    [Display(Name = "Label15", Description = "Detail15")]
    Value15 = 45,
    [Display(Name = "Label14", Description = "Detail14")]
    Value14 = 42,
    [Display(Name = "Label13", Description = "Detail13")]
    Value13 = 39,
    [Display(Name = "Label12", Description = "Detail12")]
    Value12 = 36,
    [Display(Name = "Label11", Description = "Detail11")]
    Value11 = 33,
    [Display(Name = "Value159", Description = "Detail10")]
    Value10 = 30,
    [Display(Name = "Label9", Description = "Detail9")]
    Value9 = 27,
    [EnumTransformValue(ValueOverride = "0")]
    [Display(Name = "Label8", Description = "Detail8")]
    Value8 = 24,
    [Display(Name = "Label7", Description = "Detail7")]
    Value7 = 21,
    [Display(Name = "Label6", Description = "Detail6")]
    Value6 = Alias,
    [EnumOmitValue(Exclude = EnumOmitExclude.TryParse | EnumOmitExclude.IsDefined)]
    [Display(Name = "Label5", Description = "Detail5")]
    Value5 = 15,
    [EnumTransformValue(ValueOverride = "case")]
    [Display(Name = "Label4", Description = "Detail4")]
    Value4 = 12,
    [EnumTransformValue(ValueOverride = "Case")]
    [Display(Name = "Label3", Description = "Detail3")]
    Value3 = 9,
    [EnumTransformValue(ValueOverride = "Duplicate")]
    [Display(Name = "Label2", Description = "Detail2")]
    Value2 = 6,
    [EnumTransformValue(ValueOverride = "Duplicate")]
    [Display(Name = "Label1", Description = "Detail1")]
    Value1 = 3,
    [Display(Name = "Label0", Description = "Detail0")]
    Value0 = 0
}