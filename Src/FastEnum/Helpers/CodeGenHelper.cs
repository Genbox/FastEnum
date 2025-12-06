using System.Globalization;

namespace Genbox.FastEnum.Helpers;

internal static class CodeGenHelper
{
    internal static string Indent(int amount) => new string(' ', amount * 4);

    internal static string EscapeString(string value)
    {
        StringBuilder sb = new StringBuilder(value.Length);

        foreach (char c in value)
        {
            //The switch is constructed specifically to call optimized overloads (char/string) on StringBuilder
            string? str = c switch
            {
                '"' => @"\""",
                '\\' => @"\\",
                '\0' => @"\0",
                '\n' => @"\n",
                '\r' => @"\r",
                '\t' => @"\t",
                _ => null
            };

            if (str == null)
                sb.Append(c);
            else
                sb.Append(str);
        }

        return sb.ToString();
    }

    internal static string FormatPrimitive(object value) => value switch
    {
        sbyte sb => sb.ToString(CultureInfo.InvariantCulture),
        byte b => b.ToString(CultureInfo.InvariantCulture),
        short s => s.ToString(CultureInfo.InvariantCulture),
        ushort us => us.ToString(CultureInfo.InvariantCulture),
        int i => i.ToString(CultureInfo.InvariantCulture),
        uint ui => ui.ToString(CultureInfo.InvariantCulture) + "U",
        long l => l.ToString(CultureInfo.InvariantCulture) + "L",
        ulong ul => ul.ToString(CultureInfo.InvariantCulture) + "UL",
        _ => throw new InvalidOperationException("Unsupported literal type")
    };
}