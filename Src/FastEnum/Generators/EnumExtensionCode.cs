namespace Genbox.FastEnum.Generators;

internal static class EnumExtensionCode
{
    public static string Generate(EnumSpec es)
    {
        FastEnumData op = es.Data;

        string? ns = op.ExtensionClassNamespace ?? es.Namespace;
        string cn = op.EnumNameOverride ?? es.Name;
        string en = op.ExtensionClassName ?? cn + "Extensions";
        string sn = es.Namespace == null ? "global::" + es.FullyQualifiedName : es.FullyQualifiedName;
        string vi = op.ExtensionClassVisibility == Visibility.Inherit ? (es.AccessChain[0] == Accessibility.Public ? "public" : "internal") : op.ExtensionClassVisibility.ToString().ToLowerInvariant();
        string ut = es.UnderlyingType;
        string ef = (op.EnumsClassNamespace ?? es.Namespace) != null ? $"{op.EnumsClassNamespace ?? es.Namespace}.{cn}Format" : $"{cn}Format";

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
                    using System;
                    using System.Diagnostics.CodeAnalysis;
                    {{(ns != null ? $"\nnamespace {ns};\n" : null)}}
                    {{vi}} static partial class {{en}}
                    {
                        public static string GetString(this {{sn}} value) => {{(containsDuplicateValue ? "value.ToString();" : $$"""
                                                                                                                                 value switch
                                                                                                                                     {
                                                                                                                                         {{GetString()}}
                                                                                                                                         _ => value.ToString()
                                                                                                                                     };
                                                                                                                                 """)}}

                        public static string GetString(this {{sn}} value, {{ef}} format = {{ef}}.Default)
                        {
                            {{GetStringWithFormat()}}
                        }

                        public static bool TryGetUnderlyingValue(this {{sn}} value, out {{ut}} underlyingValue)
                        {
                            {{PrintSwitch(TryGetUnderlyingValue(), containsDuplicateValue)}}
                            underlyingValue = default;
                            return false;
                        }

                        public static {{ut}} GetUnderlyingValue(this {{sn}} value)
                        {
                            if (!TryGetUnderlyingValue(value, out {{ut}} underlyingValue))
                                throw new ArgumentOutOfRangeException($"Invalid value: {value}");

                            return underlyingValue;
                        }
                    """);

        if (es.HasDisplay)
        {
            sb.Append($$"""


                            public static bool TryGetDisplayName(this {{sn}} value,
                        #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                        [NotNullWhen(true)]
                        #endif
                        out string? displayName)
                            {
                                {{PrintSwitch(TryGetDisplayName())}}
                                displayName = null;
                                return false;
                            }

                            public static string GetDisplayName(this {{sn}} value)
                            {
                                if (!TryGetDisplayName(value, out string? displayName))
                                    throw new ArgumentOutOfRangeException($"Invalid value: {value}");

                                return displayName!;
                            }
                        """);
        }

        if (es.HasDescription)
        {
            sb.Append($$"""


                            public static bool TryGetDescription(this {{sn}} value,
                        #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                        [NotNullWhen(true)]
                        #endif
                        out string? description)
                            {
                                {{PrintSwitch(TryGetDescription())}}
                                description = null;
                                return false;
                            }

                            public static string GetDescription(this {{sn}} value)
                            {
                                if (!TryGetDescription(value, out string? description))
                                    throw new ArgumentOutOfRangeException($"Invalid value: {value}");

                                return description!;
                            }
                        """);
        }

        if (es.HasFlags)
        {
            sb.Append($"""


                           public static bool IsFlagSet(this {sn} value, {sn} flag) => (({ut})value & ({ut})flag) == ({ut})flag;
                       """);
        }

        sb.Append("\n}");
        return StringBuilderPool.ReturnGetString(sb);

        string GetStringWithFormat()
        {
            StringBuilder sb2 = StringBuilderPool.Rent();

            bool hasDisplayNames = es.HasDisplay && es.Members.Any(x => x.DisplayData?.Name != null);
            bool hasDescriptions = es.HasDescription && es.Members.Any(x => x.DisplayData?.Description != null);
            bool hasOmit = es.Members.Any(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true);

            if (hasDisplayNames)
            {
                sb2.Append($"if ((format & {ef}.DisplayName) == {ef}.DisplayName)\n        {{\n");

                foreach (EnumMemberSpec em in es.Members)
                {
                    if (em.DisplayData?.Name == null)
                        continue;

                    string display = EscapeString(em.DisplayData.Name);
                    sb2.Append($"            if (value == {sn}.{em.Name}) return \"{display}\";\n");
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

                    string description = EscapeString(em.DisplayData.Description);
                    sb2.Append($"            if (value == {sn}.{em.Name}) return \"{description}\";\n");
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

                string transformed = TransformHelper.TransformName(es, em);
                sb2.Append($"            if (value == {sn}.{em.Name}) return \"{EscapeString(transformed)}\";\n");
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

            sb2.Append($"            return (({ut})value).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);\n");
            sb2.Append("        }\n\n        ");

            sb2.Append("return value.ToString();");

            return StringBuilderPool.ReturnGetString(sb2);
        }

        string GetString()
        {
            StringBuilder sb2 = StringBuilderPool.Rent(8192);

            for (int i = 0; i < es.Members.Length; i++)
            {
                EnumMemberSpec em = es.Members[i];
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                {
                    sb2.Append(sn).Append('.').Append(em.Name).Append(" => string.Empty,");

                    if (i < es.Members.Length - 1)
                        sb2.Append("\n        ");

                    continue;
                }

                string transformed = TransformHelper.TransformName(es, em);
                sb2.Append(sn).Append('.').Append(em.Name).Append(" => \"").Append(EscapeString(transformed)).Append("\",");

                if (i < es.Members.Length - 1)
                    sb2.Append("\n        ");
            }

            return StringBuilderPool.ReturnGetString(sb2);
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