namespace Genbox.FastEnum.Generators;

internal static class EnumFormatCode
{
    internal static string Generate(EnumSpec es)
    {
        FastEnumData op = es.Data;

        string? ns = es.Data.EnumsClassNamespace ?? es.Namespace; //We use the same namespace as the Enums class
        string cn = es.Data.EnumNameOverride ?? es.Name;
        string vi = op.EnumsClassVisibility == Visibility.Inherit ? (es.AccessChain[0] == Accessibility.Public ? "public" : "internal") : op.EnumsClassVisibility.ToString().ToLowerInvariant();

        string res = $$"""
                       {{(ns != null ? "\nnamespace " + ns + ";\n" : null)}}
                       /// <summary>Specifies the representations used to parse and format <see cref="{{es.FullyQualifiedName}}"/> values.</summary>
                       [global::System.Flags]
                       {{vi}} enum {{cn}}Format : byte
                       {
                           /// <summary>Do not use any representation.</summary>
                           None = 0,

                           /// <summary>Use generated member names.</summary>
                           Name = 1,

                           /// <summary>Use underlying numeric values.</summary>
                           Value = 2,
                       """;

        if (es.HasDisplay)
        {
            res += """

                       /// <summary>Use display names.</summary>
                       DisplayName = 4,
                   """;
        }

        if (es.HasDescription)
        {
            res += """

                       /// <summary>Use descriptions.</summary>
                       Description = 8,
                   """;
        }

        res += """

                   /// <summary>Use the default representations.</summary>
                   Default = Name | Value
               }
               """;
        return res;
    }
}