namespace Genbox.FastEnum.Generators;

internal static class EnumExtensionCode
{
    public static string Generate(EnumSpec es)
    {
        FastEnumData op = es.Data;

        string? ns = op.ExtensionClassNamespace ?? es.Namespace;
        string cn = op.EnumNameOverride ?? es.Name;
        string en = op.ExtensionClassName ?? cn + "Extensions";
        string sn = es.FullyQualifiedName;
        string inheritedVisibility = es.AccessChain[0] == Accessibility.Public ? "public" : "internal";
        string vi = op.ExtensionClassVisibility == Visibility.Inherit ? inheritedVisibility : op.ExtensionClassVisibility.ToString().ToLowerInvariant();
        string ut = es.UnderlyingType;
        string ef = (op.EnumsClassNamespace ?? es.Namespace) != null ? $"global::{op.EnumsClassNamespace ?? es.Namespace}.{cn}Format" : $"global::{cn}Format";

        bool containsDuplicateValue = false;
        HashSet<object> values = new HashSet<object>();

        foreach (EnumMemberSpec em in es.Members)
        {
            if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) == true)
                continue;

            if (!values.Add(em.Value))
            {
                containsDuplicateValue = true;
                break;
            }
        }

        StringBuilder sb = StringBuilderPool.Rent(16384);
        sb.Append($$"""
                    {{(ns != null ? $"\nnamespace {ns};\n" : null)}}
                    /// <summary>Provides generated extension methods for <see cref="{{sn}}"/>.</summary>
                    {{vi}} static partial class {{en}}
                    {
                        /// <summary>Gets the generated string representation of an enum value.</summary>
                        /// <param name="value">The enum value.</param>
                        /// <returns>The generated string representation.</returns>
                        public static string GetString(this {{sn}} value)
                        {
                            {{GetString()}}
                        }

                        /// <summary>Gets the string representation of an enum value using the specified formats.</summary>
                        /// <param name="value">The enum value.</param>
                        /// <param name="format">The formats to use.</param>
                        /// <returns>The formatted string representation.</returns>
                        public static string GetString(this {{sn}} value, {{ef}} format = {{ef}}.Default)
                        {
                            {{GetStringWithFormat()}}
                        }

                        /// <summary>Attempts to get the underlying numeric value of an enum value.</summary>
                        /// <param name="value">The enum value.</param>
                        /// <param name="underlyingValue">When this method returns, contains the underlying value if the lookup succeeded.</param>
                        /// <returns><see langword="true"/> if the lookup succeeded; otherwise, <see langword="false"/>.</returns>
                        public static bool TryGetUnderlyingValue(this {{sn}} value, out {{ut}} underlyingValue)
                        {
                            {{PrintSwitch(TryGetUnderlyingValue(), containsDuplicateValue)}}
                            underlyingValue = default;
                            return false;
                        }

                        /// <summary>Gets the underlying numeric value of an enum value.</summary>
                        /// <param name="value">The enum value.</param>
                        /// <returns>The underlying value.</returns>
                        /// <exception cref="global::System.ArgumentOutOfRangeException"><paramref name="value"/> is not included in the generated metadata.</exception>
                        public static {{ut}} GetUnderlyingValue(this {{sn}} value)
                        {
                            if (!TryGetUnderlyingValue(value, out {{ut}} underlyingValue))
                                throw new global::System.ArgumentOutOfRangeException($"Invalid value: {value}");

                            return underlyingValue;
                        }
                    """);

        if (es.HasDisplay)
        {
            sb.Append($$"""


                            /// <summary>Attempts to get the display name of an enum value.</summary>
                            /// <param name="value">The enum value.</param>
                            /// <param name="displayName">When this method returns, contains the display name if the lookup succeeded.</param>
                            /// <returns><see langword="true"/> if a display name was found; otherwise, <see langword="false"/>.</returns>
                            public static bool TryGetDisplayName(this {{sn}} value,
                        #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
                        #endif
                        out string? displayName)
                            {
                                {{PrintSwitch(TryGetDisplayName())}}
                                displayName = null;
                                return false;
                            }

                            /// <summary>Gets the display name of an enum value.</summary>
                            /// <param name="value">The enum value.</param>
                            /// <returns>The display name.</returns>
                            /// <exception cref="global::System.ArgumentOutOfRangeException"><paramref name="value"/> does not have a display name.</exception>
                            public static string GetDisplayName(this {{sn}} value)
                            {
                                if (!TryGetDisplayName(value, out string? displayName))
                                    throw new global::System.ArgumentOutOfRangeException($"Invalid value: {value}");

                                return displayName!;
                            }
                        """);
        }

        if (es.HasDescription)
        {
            sb.Append($$"""


                            /// <summary>Attempts to get the description of an enum value.</summary>
                            /// <param name="value">The enum value.</param>
                            /// <param name="description">When this method returns, contains the description if the lookup succeeded.</param>
                            /// <returns><see langword="true"/> if a description was found; otherwise, <see langword="false"/>.</returns>
                            public static bool TryGetDescription(this {{sn}} value,
                        #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
                        #endif
                        out string? description)
                            {
                                {{PrintSwitch(TryGetDescription())}}
                                description = null;
                                return false;
                            }

                            /// <summary>Gets the description of an enum value.</summary>
                            /// <param name="value">The enum value.</param>
                            /// <returns>The description.</returns>
                            /// <exception cref="global::System.ArgumentOutOfRangeException"><paramref name="value"/> does not have a description.</exception>
                            public static string GetDescription(this {{sn}} value)
                            {
                                if (!TryGetDescription(value, out string? description))
                                    throw new global::System.ArgumentOutOfRangeException($"Invalid value: {value}");

                                return description!;
                            }
                        """);
        }

        if (es.HasFlags)
        {
            sb.Append($"""


                           /// <summary>Determines whether all bits in a flag are set on an enum value.</summary>
                           /// <param name="value">The enum value to test.</param>
                           /// <param name="flag">The flag to test.</param>
                           /// <returns><see langword="true"/> if all bits in <paramref name="flag"/> are set; otherwise, <see langword="false"/>.</returns>
                           public static bool IsFlagSet(this {sn} value, {sn} flag) => (({ut})value & ({ut})flag) == ({ut})flag;
                       """);
        }

        sb.Append("\n}");
        return StringBuilderPool.ReturnGetString(sb);

        string GetStringWithFormat()
        {
            StringBuilder sb2 = StringBuilderPool.Rent();

            bool hasDisplayNames = es.HasDisplay && Array.Exists(es.Members, x => x.DisplayData?.Name != null);
            bool hasDescriptions = es.HasDescription && Array.Exists(es.Members, x => x.DisplayData?.Description != null);
            bool hasOmit = Array.Exists(es.Members, x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true);

            if (hasDisplayNames)
            {
                sb2.Append($"if ((format & {ef}.DisplayName) == {ef}.DisplayName)\n        {{\n");

                foreach (EnumMemberSpec em in es.Members)
                {
                    if (em.DisplayData?.Name == null)
                        continue;

                    sb2.Append($"            if (value == {sn}.{em.Name}) return \"{EscapeString(em.DisplayData.Name)}\";\n");
                }

                sb2.Append("        }\n\n        ");
            }

            if (hasDescriptions)
            {
                sb2.Append($"if ((format & {ef}.Description) == {ef}.Description)\n        {{\n");

                foreach (EnumMemberSpec em in es.Members)
                {
                    if (em.DisplayData?.Description == null)
                        continue;

                    sb2.Append($"            if (value == {sn}.{em.Name}) return \"{EscapeString(em.DisplayData.Description)}\";\n");
                }

                sb2.Append("        }\n\n        ");
            }

            sb2.Append($"if ((format & {ef}.Name) == {ef}.Name)\n        {{\n");

            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                {
                    sb2.Append($"            if (value == {sn}.{em.Name}) return string.Empty;\n");
                    continue;
                }

                sb2.Append($"            if (value == {sn}.{em.Name}) return \"{EscapeString(TransformHelper.TransformName(es, em))}\";\n");
            }

            sb2.Append("        }\n\n        ");

            sb2.Append($"if ((format & {ef}.Value) == {ef}.Value)\n        {{\n");

            if (hasOmit)
            {
                foreach (EnumMemberSpec em in es.Members)
                {
                    if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                        sb2.Append($"            if (value == {sn}.{em.Name}) return string.Empty;\n");
                }
            }

            sb2.Append($"            return (({ut})value).ToString(global::System.Globalization.NumberFormatInfo.InvariantInfo);\n");
            sb2.Append("        }\n\n        ");

            sb2.Append("return value.ToString();");

            return StringBuilderPool.ReturnGetString(sb2);
        }

        string GetString()
        {
            if (containsDuplicateValue)
            {
                // If there are no omissions or transforms, we can just return the value.
                if (Array.TrueForAll(es.Members, x => x.OmitValueData == null && x.TransformValueData == null))
                    return "return value.ToString();";

                StringBuilder sb2 = StringBuilderPool.Rent();

                foreach (EnumMemberSpec em in es.Members)
                {
                    if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                    {
                        sb2.Append($"            if (value == {sn}.{em.Name}) return string.Empty;\n");
                        continue;
                    }

                    sb2.Append($"            if (value == {sn}.{em.Name}) return \"{EscapeString(TransformHelper.TransformName(es, em))}\";\n");
                }

                sb2.Append("            return value.ToString();");
                return StringBuilderPool.ReturnGetString(sb2);
            }

            StringBuilder sb3 = StringBuilderPool.Rent();
            sb3.Append("return value switch\n        {\n            ");

            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                {
                    sb3.Append(sn).Append('.').Append(em.Name).Append(" => string.Empty,\n            ");
                    continue;
                }

                sb3.Append(sn).Append('.').Append(em.Name).Append(" => \"").Append(EscapeString(TransformHelper.TransformName(es, em))).Append("\",\n            ");
            }

            sb3.Append("_ => value.ToString()\n        };");
            return StringBuilderPool.ReturnGetString(sb3);
        }

        IEnumerable<string> TryGetUnderlyingValue()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) == true)
                    continue;

                //We default to doing a fast comparison using enum values (which is basically just integers), but in the case we have a flags enum with a duplicate value
                //we must fall back to using string comparisons, otherwise there will be duplicate branches in the switch.
                if (containsDuplicateValue)
                {
                    yield return $"""
                                              case "{em.Name}":
                                                  underlyingValue = {FormatPrimitive(em.Value)};
                                                  return true;
                                  """;
                }
                else
                {
                    yield return $"""
                                              case {sn}.{em.Name}:
                                                  underlyingValue = {FormatPrimitive(em.Value)};
                                                  return true;
                                  """;
                }
            }
        }

        IEnumerable<string> TryGetDisplayName()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetDisplayName) == true)
                    continue;

                if (em.DisplayData?.Name == null)
                    continue;

                yield return $"""
                                          case {sn}.{em.Name}:
                                              displayName = "{EscapeString(em.DisplayData.Name)}";
                                              return true;
                              """;
            }
        }

        IEnumerable<string> TryGetDescription()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetDescription) == true)
                    continue;

                if (em.DisplayData?.Description == null)
                    continue;

                yield return $"""
                                          case {sn}.{em.Name}:
                                              description = "{EscapeString(em.DisplayData.Description)}";
                                              return true;
                              """;
            }
        }

        static string PrintSwitch(IEnumerable<string> cases, bool stringComparison = false)
        {
            string[] arr = cases.ToArray();

            if (arr.Length == 0)
                return string.Empty;

            StringBuilder sb = StringBuilderPool.Rent();
            sb.AppendLine(stringComparison ? "switch (value.ToString())" : "switch (value)");
            sb.Append(Indent(2)).Append('{');
            sb.AppendLine();

            for (int i = 0; i < arr.Length; i++)
            {
                sb.Append(arr[i]);

                if (i != arr.Length - 1)
                    sb.AppendLine();
            }

            sb.AppendLine();
            sb.Append(Indent(2)).Append('}');

            return StringBuilderPool.ReturnGetString(sb);
        }
    }
}