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

        PrintArray("Transformed names:", Enums.MyEnumWithTransforms.GetMemberNames());
    }

    [EnumSourceGen]
    [Flags]
    internal enum MyEnum
    {
        [Display(Name = "Value1Name", Description = "Value1Description")]
        Value1 = 0,
        [EnumConfig(Omit = true)]
        DontShowMe
    }

    [EnumSourceGen]
    internal enum MyEnumWithTransforms
    {
        [EnumConfig(SimpleTransform = EnumTransform.UpperCase)]
        imuppercase,
        [EnumConfig(AdvancedTransform = "_LU_____U_U________")]
        MYcasingiscorrected,
        [EnumConfig(AdvancedTransform = "/^/Very/")]
        SimpleReplacement,
        [EnumConfig(Omit = true)]
        DontShowMe
    }
}