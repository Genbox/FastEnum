using Genbox.FastEnum.Spec;

namespace Genbox.FastEnum.Generators;

internal static class EnumFormatCode
{
    internal static string Generate(EnumSpec es)
    {
        string? ns = es.Data.EnumsClassNamespace ?? es.Namespace; //We use the same namespace as the Enums class
        string cn = es.Data.EnumNameOverride ?? es.Name;

        string res = $$"""
using System;
{{(ns != null ? "\nnamespace " + ns + ";\n" : null)}}
[Flags]
public enum {{cn}}Format : byte
{
    None = 0,
    Name = 1,
    Value = 2,
""";

        if (es.HasDisplay)
        {
            res += """

    DisplayName = 4,
""";
        }

        if (es.HasDescription)
        {
            res += """

    Description = 8,
""";
        }

        res += """

    Default = Name | Value
}
""";
        return res;
    }
}