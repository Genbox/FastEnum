using System.ComponentModel.DataAnnotations;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
public enum EscapedEnum
{
    [EnumOmitValue]
    None = 0,
    [Display(Name = "C:\\Path\\File\"Name", Description = "Line1\\Line2")]
    [EnumTransformValue(ValueOverride = "Val\"With\\Slash")]
    Value1 = 42
}