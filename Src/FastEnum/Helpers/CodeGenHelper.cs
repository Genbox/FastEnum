using System.Globalization;

namespace Genbox.FastEnum.Helpers;

internal static class CodeGenHelper
{
    internal static string Indent(int amount) => new string(' ', amount * 4);

    internal static string IndentFollowingLines(string text, int amount)
    {
        string indent = Indent(amount);
        return string.Join("\n", text.Split('\n').Select((line, index) =>
            index == 0 || line.Length == 0 ? line : indent + line));
    }

    internal static string FormatStringLiteral(string value) => $"\"{EscapeString(value)}\"";

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
                '\u0085' => @"\u0085",
                '\u2028' => @"\u2028",
                '\u2029' => @"\u2029",
                _ => null
            };

            if (str == null)
                sb.Append(c);
            else
                sb.Append(str);
        }

        return sb.ToString();
    }

    internal static string FormatPrimitive(object value, bool outputTypeLabel = true) => value switch
    {
        sbyte sb => sb.ToString(CultureInfo.InvariantCulture),
        byte b => b.ToString(CultureInfo.InvariantCulture),
        short s => s.ToString(CultureInfo.InvariantCulture),
        ushort us => us.ToString(CultureInfo.InvariantCulture),
        int i => i.ToString(CultureInfo.InvariantCulture),
        uint ui => ui.ToString(CultureInfo.InvariantCulture) + (outputTypeLabel ? "U" : ""),
        long l => l.ToString(CultureInfo.InvariantCulture) + (outputTypeLabel ? "L" : ""),
        ulong ul => ul.ToString(CultureInfo.InvariantCulture) + (outputTypeLabel ? "UL" : ""),
        _ => throw new InvalidOperationException("Unsupported literal type")
    };

    internal static ulong ToUInt64(object value) => value switch
    {
        byte b => b,
        sbyte sb => unchecked((ulong)sb),
        short s => unchecked((ulong)s),
        ushort us => us,
        int i => unchecked((ulong)i),
        uint ui => ui,
        long l => unchecked((ulong)l),
        ulong ul => ul,
        _ => throw new InvalidOperationException("Unsupported enum underlying type")
    };
}