// DisplayAttribute Name/Description should flow through and escape correctly

namespace Some.Namespace.Here;

[FastEnum]
public enum MyEnum
{
    [Display(Name = "myname1")] // Name only
    Value1,

    [Display(Name = "myname2\0<-nullbyte", Description = "mydescription2\0<-nullbyte")]
    Value2,

    [Display(Name = "myname3\t<-tab", Description = "mydescription3\t<-tab")]
    Value3,

    [Display(Name = "myname4\"<-qoute", Description = "mydescription4\"<-qoute")]
    Value4,

    [Display(Description = "mydescription5")] // Description only
    Value5,
}