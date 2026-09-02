namespace Genbox.FastEnum.Generators;

internal static class EnumExtensionCode
{
    public static string Generate(EnumSpec spec)
    {
        FastEnumData options = spec.Data;

        string? namespaceName = options.ExtensionClassNamespace ?? spec.Namespace;
        string extensionName = options.ExtensionClassName ?? $"{spec.Name}Extensions";
        string enumName = spec.FullyQualifiedName;
        string inheritedVisibility = spec.AccessChain[0] == Accessibility.Public ? "public" : "internal";
        string visibility = options.ExtensionClassVisibility == Visibility.Inherit ? inheritedVisibility : options.ExtensionClassVisibility.ToString().ToLowerInvariant();
        string underlyingType = spec.UnderlyingType;
        string? formatNamespace = options.EnumsClassNamespace ?? spec.Namespace;
        string enumFormat = formatNamespace != null ? $"global::{formatNamespace}.{spec.Name}Format" : $"global::{spec.Name}Format";

        HashSet<object> values = new HashSet<object>();
        bool containsDuplicateValue = spec.Members.Where(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) != true)
                                          .Any(x => !values.Add(x.Value));

        return $$"""
                 {{(namespaceName != null ? $"\nnamespace {namespaceName};\n" : null)}}
                 /// <summary>Provides generated extension methods for <see cref="{{enumName}}"/>.</summary>
                 {{visibility}} static partial class {{extensionName}}
                 {
                     /// <summary>Gets the generated string representation of an enum value.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <returns>The generated string representation.</returns>
                     public static string GetString(this {{enumName}} value)
                     {
                         {{GetString()}}
                     }

                     /// <summary>Gets the string representation of an enum value using the specified formats.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <param name="format">The formats to use.</param>
                     /// <returns>The formatted string representation.</returns>
                     public static string GetString(this {{enumName}} value, {{enumFormat}} format = {{enumFormat}}.Default)
                     {
                         {{GetStringWithFormat()}}
                     }

                     /// <summary>Attempts to get the underlying numeric value of an enum value.</summary>
                     /// <param name="value">The enum value.</param>
                     /// <param name="underlyingValue">When this method returns, contains the underlying value if the lookup succeeded.</param>
                     /// <returns><see langword="true"/> if the lookup succeeded; otherwise, <see langword="false"/>.</returns>
                     public static bool TryGetUnderlyingValue(this {{enumName}} value, out {{underlyingType}} underlyingValue)
                     {
                         {{PrintSwitch(TryGetUnderlyingValue(), containsDuplicateValue)}}
                         underlyingValue = default;
                         return false;
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
                     }{{GetDisplayMethods()}}{{GetDescriptionMethods()}}{{GetFlagsMethod()}}
                 }
                 """;

        string GetDisplayMethods() => GetMetadataMethods(spec.HasDisplay, "display name", "DisplayName", "displayName", x => x.DisplayData?.Name, EnumOmitExclude.TryGetDisplayName);

        string GetDescriptionMethods() => GetMetadataMethods(spec.HasDescription, "description", "Description", "description", x => x.DisplayData?.Description, EnumOmitExclude.TryGetDescription);

        string GetMetadataMethods(bool enabled, string label, string suffix, string variable, Func<EnumMemberSpec, string?> getText, EnumOmitExclude exclusion) => !enabled ? string.Empty : $$"""


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
                                {{PrintSwitch(GetMetadataCases(getText, exclusion, variable))}}
                                {{variable}} = null;
                                return false;
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

        string GetFlagsMethod() => !spec.HasFlags ? string.Empty : $"""


                           /// <summary>Determines whether all bits in a flag are set on an enum value.</summary>
                           /// <param name="value">The enum value to test.</param>
                           /// <param name="flag">The flag to test.</param>
                           /// <returns><see langword="true"/> if all bits in <paramref name="flag"/> are set; otherwise, <see langword="false"/>.</returns>
                           public static bool IsFlagSet(this {enumName} value, {enumName} flag) => (({underlyingType})value & ({underlyingType})flag) == ({underlyingType})flag;
                       """;

        string GetStringWithFormat()
        {
            bool hasDisplayNames = spec.HasDisplay && Array.Exists(spec.Members, x => x.DisplayData?.Name != null);
            bool hasDescriptions = spec.HasDescription && Array.Exists(spec.Members, x => x.DisplayData?.Description != null);

            return $"""
                    {string.Join("\n\n        ", GetFormatBlocks())}

                            return value.ToString();
                    """;

            IEnumerable<string> GetFormatBlocks()
            {
                if (hasDisplayNames)
                    yield return GetFormatBlock("DisplayName", GetMetadataChecks(x => x.DisplayData?.Name));

                if (hasDescriptions)
                    yield return GetFormatBlock("Description", GetMetadataChecks(x => x.DisplayData?.Description));

                yield return GetFormatBlock("Name", GetNameChecks());
                yield return GetFormatBlock("Value", GetValueStatements());
            }

            string GetFormatBlock(string format, IEnumerable<string> statements)
            {
                string[] arr = statements.ToArray();

                if (arr.Length == 0)
                {
                    return $$"""
                             if ((format & {{enumFormat}}.{{format}}) == {{enumFormat}}.{{format}})
                                     {
                                     }
                             """;
                }

                return $$"""
                         if ((format & {{enumFormat}}.{{format}}) == {{enumFormat}}.{{format}})
                                 {
                         {{string.Join("\n", arr)}}
                                 }
                         """;
            }

            IEnumerable<string> GetMetadataChecks(Func<EnumMemberSpec, string?> getValue)
            {
                foreach (EnumMemberSpec em in spec.Members)
                {
                    string? value = getValue(em);

                    if (value != null)
                        yield return $"            if (value == {enumName}.{em.EmittedIdentifier}) return \"{EscapeString(value)}\";";
                }
            }

            IEnumerable<string> GetNameChecks()
            {
                foreach (EnumMemberSpec em in spec.Members)
                    yield return $"            if (value == {enumName}.{em.EmittedIdentifier}) return {GetNameResult(em)};";
            }

            IEnumerable<string> GetValueStatements()
            {
                foreach (EnumMemberSpec em in spec.Members)
                {
                    if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                        yield return $"            if (value == {enumName}.{em.EmittedIdentifier}) return string.Empty;";
                }

                yield return $"            return (({underlyingType})value).ToString(global::System.Globalization.NumberFormatInfo.InvariantInfo);";
            }
        }

        string GetString()
        {
            if (containsDuplicateValue)
            {
                // If there are no omissions or transforms, we can just return the value.
                if (Array.TrueForAll(spec.Members, x => x.OmitValueData == null && x.TransformValueData == null))
                    return "return value.ToString();";

                return $"""
                        {string.Join("\n", GetDuplicateValueChecks())}
                                    return value.ToString();
                        """;
            }

            return $$"""
                     return value switch
                             {
                     {{string.Join("\n", GetSwitchArms())}}
                             };
                     """;

            IEnumerable<string> GetDuplicateValueChecks()
            {
                foreach (EnumMemberSpec em in spec.Members)
                    yield return $"            if (value == {enumName}.{em.EmittedIdentifier}) return {GetNameResult(em)};";
            }

            IEnumerable<string> GetSwitchArms()
            {
                foreach (EnumMemberSpec em in spec.Members)
                    yield return $"            {enumName}.{em.EmittedIdentifier} => {GetNameResult(em)},";

                yield return "            _ => value.ToString()";
            }
        }

        IEnumerable<string> TryGetUnderlyingValue()
        {
            HashSet<object> handledValues = new HashSet<object>(spec.Members.Where(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) != true)
                                                                            .Select(x => x.Value));

            foreach (EnumMemberSpec em in spec.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) == true)
                    continue;

                // Duplicate enum values need string comparisons to avoid duplicate switch branches.
                string caseValue = containsDuplicateValue ? $"\"{em.Name}\"" : $"{enumName}.{em.EmittedIdentifier}";
                yield return $"""
                                          case {caseValue}:
                                              underlyingValue = {FormatPrimitive(em.Value)};
                                              return true;
                              """;
            }

            if (spec.HasFlags)
            {
                foreach (EnumMemberSpec em in spec.Members)
                {
                    if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) != true || !handledValues.Add(em.Value))
                        continue;

                    string caseValue = containsDuplicateValue ? $"\"{em.Name}\"" : $"{enumName}.{em.EmittedIdentifier}";
                    yield return $"""
                                              case {caseValue}:
                                                  break;
                                  """;
                }

                ulong mask = spec.Members.Where(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) != true)
                                 .Aggregate(0UL, (value, member) => value | ToUInt64(member.Value));

                // Valid Flags combinations need not have a separately declared alias.
                yield return $$"""
                                           default:
                                               if (unchecked((({{underlyingType}}){{mask}}UL & ({{underlyingType}})value) == ({{underlyingType}})value))
                                               {
                                                   underlyingValue = ({{underlyingType}})value;
                                                   return true;
                                               }

                                               break;
                               """;
            }
        }

        IEnumerable<string> GetMetadataCases(Func<EnumMemberSpec, string?> getText, EnumOmitExclude exclusion, string resultName)
        {
            foreach (EnumMemberSpec em in spec.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(exclusion) == true)
                    continue;

                string? text = getText(em);
                if (text == null)
                    continue;

                yield return $"""
                                          case {enumName}.{em.EmittedIdentifier}:
                                              {resultName} = "{EscapeString(text)}";
                                              return true;
                              """;
            }
        }

        static string PrintSwitch(IEnumerable<string> cases, bool stringComparison = false)
        {
            string[] arr = cases.ToArray();

            if (arr.Length == 0)
                return string.Empty;

            return $$"""
                     {{(stringComparison ? "switch (value.ToString())" : "switch (value)")}}
                             {
                     {{string.Join("\n", arr)}}
                             }
                     """;
        }

        string GetNameResult(EnumMemberSpec em)
        {
            if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                return "string.Empty";

            return $"\"{EscapeString(TransformHelper.TransformName(spec, em))}\"";
        }

    }
}