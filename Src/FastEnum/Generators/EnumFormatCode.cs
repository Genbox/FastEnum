namespace Genbox.FastEnum.Generators;

internal static class EnumFormatCode
{
    internal static string Generate(EnumSpec spec)
    {
        FastEnumData options = spec.Data;
        string? namespaceName = options.EnumsClassNamespace ?? spec.Namespace;
        bool isPublicEnum = spec.IsPubliclyAccessible;
        bool isEnumsClassPublic = options.EnumsClassVisibility == Visibility.Inherit ? isPublicEnum : options.EnumsClassVisibility == Visibility.Public;
        bool isExtensionClassPublic = options.ExtensionClassVisibility == Visibility.Inherit ? isPublicEnum : options.ExtensionClassVisibility == Visibility.Public;

        // Format enum visibility must cover every generated public API that exposes it.
        string visibility = isEnumsClassPublic || isExtensionClassPublic ? "public" : "internal";

        return $$"""
                 {{(namespaceName != null ? $"\nnamespace {namespaceName};\n" : null)}}
                 /// <summary>Specifies the representations used to parse and format <see cref="{{spec.FullyQualifiedName}}"/> values.</summary>
                 {{EnumGenerator.GeneratedCodeAttribute}}
                 [global::System.Flags]
                 {{visibility}} enum {{spec.Name}}Format : byte
                 {
                     /// <summary>Do not use any representation.</summary>
                     None = 0,

                     /// <summary>Use generated member names.</summary>
                     Name = 1,

                     /// <summary>Use underlying numeric values.</summary>
                     Value = 2,{{DisplayNameMember()}}{{DescriptionMember()}}
                     /// <summary>Use the default representations.</summary>
                     Default = Name | Value
                 }
                 """;

        string DisplayNameMember() => !spec.HasDisplay
            ? string.Empty
            : """

                  /// <summary>Use display names.</summary>
                  DisplayName = 4,
              """;

        string DescriptionMember() => !spec.HasDescription
            ? string.Empty
            : """

                  /// <summary>Use descriptions.</summary>
                  Description = 8,
              """;
    }
}