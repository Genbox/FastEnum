using System.ComponentModel.DataAnnotations;
using static Genbox.EnumSourceGen.Examples.Utilities;

namespace Genbox.EnumSourceGen.Examples;

internal static class Program
{
    private static void Main()
    {
        EnumExtensions();
        EnumClass();
        Transforms();
        OmittingValues();
    }

    private static void EnumExtensions()
    {
        PrintHeader("Enum Extensions");

        MyEnum e = MyEnum.Value1;

        //Extensions
        Console.WriteLine("String value: " + e.GetString());
        Console.WriteLine("Underlying value: " + e.GetUnderlyingValue());
        Console.WriteLine("Display name: " + e.GetDisplayName());
        Console.WriteLine("Description: " + e.GetDescription());
        Console.WriteLine("Has Value1 flag: " + e.IsFlagSet(MyEnum.Value1));
    }

    private static void EnumClass()
    {
        PrintHeader("Enum Class");

        //Enums
        Console.WriteLine("Number of members: " + Enums.MyEnum.MemberCount);
        Console.WriteLine("Is it a flags enum: " + Enums.MyEnum.IsFlagEnum);

        Console.WriteLine("Parse: " + Enums.MyEnum.Parse("Value1"));
        Console.WriteLine("TryParse success: " + Enums.MyEnum.TryParse("value1", out MyEnum val, MyEnumFormat.Default, StringComparison.OrdinalIgnoreCase) + " value: " + val);
        Console.WriteLine("Is Value1 part of the enum: " + Enums.MyEnum.IsDefined(MyEnum.Value1));
        Console.WriteLine("Is 42 part of the enum: " + Enums.MyEnum.IsDefined((MyEnum)42));

        PrintArray("Member names:", Enums.MyEnum.GetMemberNames());
        PrintArray("Member values:", Enums.MyEnum.GetMemberValues());
        PrintArray("Underlying values:", Enums.MyEnum.GetUnderlyingValues());
        PrintArray("Display names:", Enums.MyEnum.GetDisplayNames());
        PrintArray("Descriptions:", Enums.MyEnum.GetDescriptions());
    }

    private static void Transforms()
    {
        PrintHeader("Transforms");

        Console.WriteLine("\nPreset:");
        foreach (EnumWithPresetTransform value in Enums.EnumWithPresetTransform.GetMemberValues())
            Console.WriteLine(value + " -> " + value.GetString());

        Console.WriteLine("\nRegex:");
        foreach (EnumWithRegexTransform value in Enums.EnumWithRegexTransform.GetMemberValues())
            Console.WriteLine(value + " -> " + value.GetString());

        Console.WriteLine("\nCase pattern:");
        foreach (EnumWithCasePatternTransform value in Enums.EnumWithCasePatternTransform.GetMemberValues())
            Console.WriteLine(value + " -> " + value.GetString());
    }

    private static void OmittingValues()
    {
        PrintHeader("Omitting values");

        Console.WriteLine("Value1 is defined: " + Enums.EnumWithOmit.IsDefined(EnumWithOmit.Value1));
        Console.WriteLine("ThisIsOmitted is defined: " + Enums.EnumWithOmit.IsDefined(EnumWithOmit.ThisIsOmitted));
        Console.WriteLine("NotOmittedInIsDefined is defined: " + Enums.EnumWithOmit.IsDefined(EnumWithOmit.NotOmittedInIsDefined));
    }

    [Flags]
    [EnumSourceGen]
    internal enum MyEnum
    {
        [Display(Name = "Value1Name", Description = "Value1Description")]
        Value1 = 0,
        Value2
    }

    [EnumSourceGen]
    [EnumTransform(Preset = EnumTransform.UpperCase)]
    internal enum EnumWithPresetTransform
    {
        imuppercase,
        [EnumTransformValue(ValueOverride = "my-value")]
        myvalue,
    }

    [EnumSourceGen]
    [EnumTransform(Regex = "/^Enum//")] //Replace "Enum" with nothing
    internal enum EnumWithRegexTransform
    {
        EnumValue1,
        EnumValue2,
    }

    [EnumSourceGen]
    [EnumTransform(CasePattern = "U_U_U")] //Uppercase the first, third and fifth characters
    internal enum EnumWithCasePatternTransform
    {
        Value1,
        Value2,
    }

    [EnumSourceGen]
    internal enum EnumWithOmit
    {
        [Display(Name = "Value1Name", Description = "Value1Description")]
        Value1 = 0,

        [EnumOmitValue]
        ThisIsOmitted,

        [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
        NotOmittedInIsDefined
    }

    [EnumSourceGen]
    internal enum MyEnum2
    {
        [Display(Name = "Value1Name")]
        Value1
    }
}