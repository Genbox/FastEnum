using System.ComponentModel.DataAnnotations;

namespace Genbox.EnumSourceGen.Examples;

internal static class Program
{
    private static void Main(string[] args)
    {
        TestEnum e = TestEnum.Value1;

        //Extensions
        Console.WriteLine("String value: " + e.GetString());
        Console.WriteLine("Underlying value: " + e.GetUnderlyingValue());
        Console.WriteLine("Display name: " + e.GetDisplayName());
        Console.WriteLine("Description: " + e.GetDescription());
        Console.WriteLine("Has Value1 flag: " + e.IsFlagSet(TestEnum.Value1));

        //Enums
        Console.WriteLine("Number of members: " + Enums.TestEnum.MemberCount);

        Console.WriteLine("Parse: " + Enums.TestEnum.Parse("value1", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine("TryParse success: " + Enums.TestEnum.TryParse("value1", out TestEnum val, StringComparison.OrdinalIgnoreCase) + " value: " + val);
        Console.WriteLine("Is Value1 part of the enum: " + Enums.TestEnum.IsDefined(TestEnum.Value1));
        Console.WriteLine("Is 42 part of the enum: " + Enums.TestEnum.IsDefined((TestEnum)42));

        PrintArray("Member names:", Enums.TestEnum.GetMemberNames());
        PrintArray("Member values:", Enums.TestEnum.GetMemberValues());
        PrintArray("Underlying values:", Enums.TestEnum.GetUnderlyingValues());
        PrintArray("Display names:", Enums.TestEnum.GetDisplayNames());
        PrintArray("Descriptions:", Enums.TestEnum.GetDescriptions());
    }

    private static void PrintArray<T>(string title, IEnumerable<T> arr)
    {
        Console.WriteLine(title);

        foreach (T a in arr)
            Console.WriteLine("- " + a);
    }

    [EnumSourceGen]
    [Flags]
    internal enum TestEnum : byte
    {
        [Display(Name = "Value1Name", Description = "Value1Description")]
        Value1 = 0,
        [Display(Name = "DisplayNameForValue2")]
        Value2 = 1,
        [Display(Description = "Description for Value3")]
        Value3 = 2,
        Value4 = 4
    }
}