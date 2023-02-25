namespace Genbox.EnumSourceGen.Helpers;

internal static class CodeGenHelper
{
    internal static string Indent(int amount)
    {
        return new string(' ', amount * 4);
    }
}