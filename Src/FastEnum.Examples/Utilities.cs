namespace Genbox.FastEnum.Examples;

/// <summary>
/// Just a few utilities to help printing to the console
/// </summary>
internal static class Utilities
{
    internal static void PrintArray<T>(string title, IEnumerable<T> arr)
    {
        Console.WriteLine(title);

        foreach (T a in arr)
            Console.WriteLine("- " + a);
    }

    internal static void PrintHeader(string header)
    {
        Console.WriteLine();
        Console.WriteLine('#');
        Console.WriteLine("# " + header);
        Console.WriteLine('#');
    }
}