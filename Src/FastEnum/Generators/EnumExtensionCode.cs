namespace Genbox.FastEnum.Generators;

internal static class EnumExtensionCode
{
    public static string Generate(EnumSpec spec, bool includeGeneratedCodeAttribute)
    {
        FastEnumData options = spec.Data;

        string? namespaceName = options.ExtensionClassNamespace ?? spec.Namespace;
        string extensionName = options.ExtensionClassName ?? $"{spec.Name}Extensions";
        string enumName = spec.FullyQualifiedName;
        string inheritedVisibility = spec.IsPubliclyAccessible ? "public" : "internal";
        string visibility = options.ExtensionClassVisibility == Visibility.Inherit ? inheritedVisibility : options.ExtensionClassVisibility.ToString().ToLowerInvariant();
        string underlyingType = spec.UnderlyingType;
        string? formatNamespace = options.EnumsClassNamespace ?? spec.Namespace;
        string enumFormat = formatNamespace != null ? $"global::{formatNamespace}.{spec.Name}Format" : $"global::{spec.Name}Format";

        HashSet<object> values = new HashSet<object>();
        bool hasAliases = spec.Members.Any(x => !values.Add(x.Value));

        List<string> lookupFields = new List<string>();
        Dictionary<string, string> lookups = new Dictionary<string, string>(StringComparer.Ordinal);

        return $$"""
                 {{(namespaceName != null ? $"\nnamespace {namespaceName};\n" : null)}}
                 /// <summary>Provides generated extension methods for <see cref="{{enumName}}"/>.</summary>
                 {{(includeGeneratedCodeAttribute ? $"{EnumGenerator.GeneratedCodeAttribute}\n" : null)}}{{visibility}} static partial class {{extensionName}}
                 {
                     /// <summary>Gets the generated string representation of an enum value.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <returns>The generated string representation.</returns>
                     [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                     public static string GetString(this {{enumName}} value)
                     {
                         {{IndentFollowingLines(GetString(), 2)}}
                     }

                     /// <summary>Gets the string representation of an enum value using the specified formats.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <param name="format">The formats to use.</param>
                     /// <returns>The formatted string representation.</returns>
                     public static string GetString(this {{enumName}} value, {{enumFormat}} format = {{enumFormat}}.Default)
                     {
                         {{IndentFollowingLines(GetStringWithFormat(), 2)}}
                     }

                     /// <summary>Attempts to get the underlying numeric value of an enum value.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <param name="underlyingValue">When this method returns, contains the underlying value if the lookup succeeded.</param>
                     /// <returns><see langword="true"/> if the lookup succeeded; otherwise, <see langword="false"/>.</returns>
                     public static bool TryGetUnderlyingValue(this {{enumName}} value, out {{underlyingType}} underlyingValue)
                     {
                         {{IndentFollowingLines(GetUnderlyingLookup(), 2)}}
                     }

                     /// <summary>Gets the underlying numeric value of an enum value.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <returns>The underlying value.</returns>
                     /// <exception cref="global::System.ArgumentOutOfRangeException"><paramref name="value"/> is not included in the generated metadata.</exception>
                     public static {{underlyingType}} GetUnderlyingValue(this {{enumName}} value)
                     {
                         if (!TryGetUnderlyingValue(value, out {{underlyingType}} underlyingValue))
                             throw new global::System.ArgumentOutOfRangeException($"Invalid value: {value}");

                         return underlyingValue;
                     }{{GetDisplayMethods()}}{{GetDescriptionMethods()}}{{GetFlagsMethod()}}{{GetLookupFields()}}
                 }
                 """;

        string GetLookupFields() => lookupFields.Count == 0
            ? string.Empty
            : "\n\n" + string.Join("\n\n", lookupFields.Select(x => string.Join("\n", x.Split('\n').Select(line => "    " + line))));

        string GetDisplayMethods() => GetMetadataMethods(spec.HasDisplay, "display name", "DisplayName", "displayName", x => x.DisplayData?.Name, EnumOmitExclude.TryGetDisplayName);

        string GetDescriptionMethods() => GetMetadataMethods(spec.HasDescription, "description", "Description", "description", x => x.DisplayData?.Description, EnumOmitExclude.TryGetDescription);

        string GetMetadataMethods(bool enabled, string label, string suffix, string variable, Func<EnumMemberSpec, string?> getText, EnumOmitExclude exclusion) => !enabled
            ? string.Empty
            : $$"""


                    /// <summary>Attempts to get the {{label}} of an enum value.</summary>
                    /// <param name="value">The enum value.</param>
                    /// <param name="{{variable}}">When this method returns, contains the {{label}} if the lookup succeeded.</param>
                    /// <returns><see langword="true"/> if a {{label}} was found; otherwise, <see langword="false"/>.</returns>
                    public static bool TryGet{{suffix}}(this {{enumName}} value,
                #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
                #endif
                out string? {{variable}})
                    {
                        {{IndentFollowingLines(GetMetadataLookup(getText, exclusion, variable), 2)}}
                    }

                    /// <summary>Gets the {{label}} of an enum value.</summary>
                    /// <param name="value">The enum value.</param>
                    /// <returns>The {{label}}.</returns>
                    /// <exception cref="global::System.ArgumentOutOfRangeException"><paramref name="value"/> does not have a {{label}}.</exception>
                    public static string Get{{suffix}}(this {{enumName}} value)
                    {
                        if (!TryGet{{suffix}}(value, out string? {{variable}}))
                            throw new global::System.ArgumentOutOfRangeException($"Invalid value: {value}");

                        return {{variable}}!;
                    }
                """;

        string GetFlagsMethod() => !spec.HasFlags
            ? string.Empty
            : $"""


                   /// <summary>Determines whether all bits in a flag are set on an enum value.</summary>
                   /// <param name="value">The enum value to test.</param>
                   /// <param name="flag">The flag to test.</param>
                   /// <returns><see langword="true"/> if all bits in <paramref name="flag"/> are set; otherwise, <see langword="false"/>.</returns>
                   public static bool IsFlagSet(this {enumName} value, {enumName} flag) => (({underlyingType})value & ({underlyingType})flag) == ({underlyingType})flag;
               """;

        string Lookup(string name, IEnumerable<EnumMemberSpec> members, Func<EnumMemberSpec, string>? result)
        {
            // Filtering must precede alias resolution: the first applicable alias wins.
            EnumMemberSpec[] unique = members.GroupBy(x => x.Value).Select(x => x.First()).ToArray();
            StringBuilder signature = new StringBuilder(result == null ? "bool" : "string");

            foreach (EnumMemberSpec member in unique)
            {
                string entry = FormatPrimitive(member.Value) + "=" + (result?.Invoke(member) ?? "true");
                signature.Append(entry.Length).Append(':').Append(entry);
            }

            string key = signature.ToString();

            if (!lookups.TryGetValue(key, out string? expression))
            {
                expression = EnumLookupCode.Create(spec, unique, name, "value", result, lookupFields);
                lookups.Add(key, expression);
            }

            return expression;
        }

        string NameLookup() => Lookup("_stringLookup", spec.Members, GetNameResult);

        string GetStringWithFormat()
        {
            List<string> blocks = new List<string>();
            if (spec.HasDisplay && Array.Exists(spec.Members, x => x.DisplayData?.Name != null))
                AddTextBlock("DisplayName", x => x.DisplayData?.Name);
            if (spec.HasDescription && Array.Exists(spec.Members, x => x.DisplayData?.Description != null))
                AddTextBlock("Description", x => x.DisplayData?.Description);

            if (spec.Members.Length > 0)
            {
                blocks.Add(Block("Name", $$"""
                                           string? name = {{NameLookup()}};
                                           if (name != null)
                                               return name;
                                           """));
            }

            string omitted = Lookup("_stringOmittedLookup", spec.Members.Where(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true), null);
            string numericFormat = $"return (({underlyingType})value).ToString(global::System.Globalization.NumberFormatInfo.InvariantInfo);";

            if (omitted != "false")
            {
                numericFormat = $$"""
                                  if ({{omitted}})
                                      return string.Empty;
                                  {{numericFormat}}
                                  """;
            }

            blocks.Add(Block("Value", numericFormat));
            blocks.Add("return value.ToString();");
            return string.Join("\n\n", blocks);

            void AddTextBlock(string format, Func<EnumMemberSpec, string?> getText)
            {
                string? Text(EnumMemberSpec member) => member.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true ? string.Empty : getText(member);
                string lookup = Lookup($"_format{format}Lookup", spec.Members.Where(x => Text(x) != null), x => FormatStringLiteral(Text(x)!));
                blocks.Add(Block(format, $$"""
                                           string? text = {{lookup}};
                                           if (text != null)
                                               return text;
                                           """));
            }

            string Block(string format, string body) => $$"""
                                                          if ((format & {{enumFormat}}.{{format}}) == {{enumFormat}}.{{format}})
                                                          {
                                                              {{IndentFollowingLines(body, 1)}}
                                                          }
                                                          """;
        }

        string GetString()
        {
            if (spec.Members.Length == 0)
                return "return value.ToString();";

            // Preserve Enum.ToString's alias selection when no generated override applies.
            if (hasAliases && spec.TransformData == null && Array.TrueForAll(spec.Members, x => x.OmitValueData == null && x.TransformValueData == null))
                return "return value.ToString();";

            EnumMemberSpec[] members = spec.Members.GroupBy(member => member.Value).Select(group => group.First()).ToArray();
            if (EnumLookupCode.UsesSwitch(spec, members.Length))
                return "return " + EnumLookupCode.Create(spec, members, "_stringLookup", "value", GetNameResult, lookupFields, "value.ToString()") + ";";

            return $"return {NameLookup()} ?? value.ToString();";
        }

        string GetUnderlyingLookup()
        {
            const string failure = "underlyingValue = default;\nreturn false;";
            EnumMemberSpec[] included = spec.Members
                                            .Where(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) != true)
                                            .GroupBy(x => x.Value).Select(x => x.First()).ToArray();
            if (included.Length == 0)
                return failure;
            string match;

            if (spec.HasFlags)
            {
                HashSet<object> includedValues = new HashSet<object>(included.Select(x => x.Value));
                EnumMemberSpec[] excluded = spec.Members.Where(x => !includedValues.Contains(x.Value))
                                                .GroupBy(x => x.Value).Select(x => x.First()).ToArray();
                string omitted = Lookup("_underlyingExcludedLookup", excluded, null);
                ulong mask = included.Aggregate(0UL, (bits, member) => bits | ToUInt64(member.Value));
                string maskCheck = mask == 0
                    ? $"({underlyingType})value == 0"
                    : $"unchecked((({underlyingType}){mask}UL & ({underlyingType})value) == ({underlyingType})value)";

                // Every included member already satisfies the mask; only explicit exclusions need a lookup.
                match = (excluded.Length == 0 ? string.Empty : $"!({omitted}) && ") + maskCheck;
            }
            else
                match = Lookup("_underlyingLookup", included, null);

            return $$"""
                     if ({{match}})
                     {
                         underlyingValue = ({{underlyingType}})value;
                         return true;
                     }
                     {{failure}}
                     """;
        }

        string GetMetadataLookup(Func<EnumMemberSpec, string?> getText, EnumOmitExclude exclusion, string variable)
        {
            EnumMemberSpec[] members = spec.Members
                                           .Where(x => x.OmitValueData?.Exclude.HasFlag(exclusion) != true && getText(x) != null)
                                           .GroupBy(x => x.Value).Select(x => x.First()).ToArray();

            if (members.Length == 0)
            {
                return $$"""
                         {{variable}} = null;
                         return false;
                         """;
            }

            string lookup = Lookup($"_{variable}Lookup", members, x => FormatStringLiteral(getText(x)!));
            return $$"""
                     {{variable}} = {{lookup}};
                     return {{variable}} != null;
                     """;
        }

        string GetNameResult(EnumMemberSpec em)
        {
            if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                return "string.Empty";

            return FormatStringLiteral(TransformHelper.TransformName(spec, em));
        }
    }
}