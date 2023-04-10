using System.Globalization;
using System.Text;
using Genbox.EnumSourceGen.Data;
using Genbox.EnumSourceGen.Helpers;
using Genbox.EnumSourceGen.Misc;
using static Genbox.EnumSourceGen.Helpers.CodeGenHelper;

namespace Genbox.EnumSourceGen.Generators;

internal static class EnumClassCode
{
    internal static string Generate(EnumSpec es, StringBuilder sb)
    {
        sb.Clear();

        EnumSourceGenData op = es.SourceGenData;

        string? ns = op.EnumsClassNamespace ?? es.Namespace;
        string cn = op.EnumNameOverride ?? es.Name;
        string en = op.EnumsClassName ?? "Enums";
        string sn = es.FullyQualifiedName; //We always use FQN. The class name is the same as the enum name
        string vi = es.IsPublic ? "public" : "internal";
        string ut = es.UnderlyingType;
        string mc = (es.Members.Count - es.Members.Count(x => x.OmitValueData != null)).ToString(NumberFormatInfo.InvariantInfo);
        string ef = (ns != null ? ns + '.' : null) + cn + "Format";

        string res = $$"""
// <auto-generated />
#nullable enable
{{(ns != null ? "\nnamespace " + ns + ";\n" : null)}}
{{(!op.DisableEnumsWrapper ? $"public static partial class {en}\n{{" : "")}}
    {{vi}} static partial class {{cn}}
    {
        public const int MemberCount = {{mc}};
        public const bool IsFlagEnum = {{es.HasFlags.ToString().ToLowerInvariant()}};

        {{(op.DisableCache ? null : "private static string[]? _names;")}}
        public static string[] GetMemberNames()
            => {{CachedAssignment("_names")}} new[] {
                {{GetMemberNames()}}
            };

        {{(op.DisableCache ? null : $"private static {sn}[]? _values;")}}
        public static {{sn}}[] GetMemberValues()
            => {{CachedAssignment("_values")}} new[] {
                {{GetMemberValues()}}
            };

        {{(op.DisableCache ? null : $"private static {ut}[]? _underlyingValues;")}}
        public static {{ut}}[] GetUnderlyingValues()
            => {{CachedAssignment("_underlyingValues")}} new {{ut}}[] {
                {{GetUnderlyingValues()}}
            };

        public static bool TryParse(string value, out {{sn}} result, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
{{TryParse()}}

            result = default;
            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> value, out {{sn}} result, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
{{TryParse()}}

            result = default;
            return false;
        }

        public static {{sn}} Parse(ReadOnlySpan<char> value, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
            if (!TryParse(value, out {{sn}} result, format, comparison))
                throw new ArgumentOutOfRangeException($"Invalid value: {value}");

            return result;
        }

        public static {{sn}} Parse(string value, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
            if (!TryParse(value, out {{sn}} result, format, comparison))
                throw new ArgumentOutOfRangeException($"Invalid value: {value}");

            return result;
        }

        public static bool IsDefined({{sn}} input) => {{IsDefined()}};
""";

        if (es.HasDisplay)
        {
            res +=
                $$"""


        {{(op.DisableCache ? null : $"private static ({sn}, string)[]? _displayNames;")}}
        public static ({{sn}}, string)[] GetDisplayNames()
            => {{CachedAssignment("_displayNames")}} new [] {
                {{GetDisplayNames()}}
            };
""";
        }

        if (es.HasDescription)
        {
            res +=
                $$"""


        {{(op.DisableCache ? null : $"private static ({sn}, string)[]? _descriptions;")}}
        public static ({{sn}}, string)[] GetDescriptions()
            => {{CachedAssignment("_descriptions")}} new[] {
                {{GetDescriptions()}}
            };
""";
        }

        string CachedAssignment(string name) => !op.DisableCache ? $"{name} ??=" : string.Empty;

        string GetMemberNames()
        {
            sb.Clear();

            foreach (EnumMember em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.GetMemberNames))
                    continue;

                string name = TransformHelper.TransformName(em);
                sb.Append('"').Append(name).Append("\",\n").Append(Indent(4));
            }

            return sb.ToString().TrimEnd(CodeConstants.TrimChars);
        }

        string GetMemberValues()
        {
            sb.Clear();

            foreach (EnumMember em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.GetMemberValues))
                    continue;

                sb.Append(sn).Append('.').Append(em.Name).Append(",\n").Append(Indent(4));
            }

            return sb.ToString().TrimEnd(CodeConstants.TrimChars);
        }

        string GetUnderlyingValues()
        {
            sb.Clear();

            foreach (EnumMember em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.GetUnderlyingValues))
                    continue;

                sb.Append(em.Value).Append(",\n").Append(Indent(4));
            }

            return sb.ToString().TrimEnd(CodeConstants.TrimChars);
        }

        string TryParse()
        {
            sb.Clear();

            sb.Append($$"""

            if (format.HasFlag({{ef}}.Name))
            {
""");

            for (int i = 0; i < es.Members.Count; i++)
            {
                EnumMember em = es.Members[i];

                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryParse))
                    continue;

                sb.Append($$"""

                if (value.Equals("{{em.Name}}", comparison))
                {
                    result = {{sn}}.{{em.Name}};
                    return true;
                }
""");

                if (i != es.Members.Count - 1)
                    sb.AppendLine();
            }

            sb.Append("\n            }");

            sb.Append($$"""

            if (format.HasFlag({{ef}}.Value))
            {
""");

            for (int i = 0; i < es.Members.Count; i++)
            {
                EnumMember em = es.Members[i];

                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryParse))
                    continue;

                sb.Append($$"""

                if (value.Equals("{{em.Value}}", comparison))
                {
                    result = {{sn}}.{{em.Name}};
                    return true;
                }
""");

                if (i != es.Members.Count - 1)
                    sb.AppendLine();
            }

            sb.Append("\n            }");

            if (es.HasDisplay)
            {
                sb.Append($$"""

            if (format.HasFlag({{ef}}.DisplayName))
            {
""");

                for (int i = 0; i < es.Members.Count; i++)
                {
                    EnumMember em = es.Members[i];

                    if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryParse))
                        continue;

                    if (em.DisplayData?.Name != null)
                    {
                        sb.Append($$"""

                if (value.Equals("{{em.DisplayData.Name}}", comparison))
                {
                    result = {{sn}}.{{em.Name}};
                    return true;
                }
""");
                    }
                    if (i != es.Members.Count - 1)
                        sb.AppendLine();
                }

                sb.Append("\n            }");
            }

            if (es.HasDescription)
            {
                sb.Append($$"""

            if (format.HasFlag({{ef}}.Description))
            {
""");

                for (int i = 0; i < es.Members.Count; i++)
                {
                    EnumMember em = es.Members[i];

                    if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryParse))
                        continue;

                    if (em.DisplayData?.Description != null)
                    {
                        sb.Append($$"""

                if (value.Equals("{{em.DisplayData.Description}}", comparison))
                {
                    result = {{sn}}.{{em.Name}};
                    return true;
                }
""");
                    }

                    if (i != es.Members.Count - 1)
                        sb.AppendLine();
                }

                sb.Append("\n            }");
            }

            return sb.ToString();
        }

        string GetDisplayNames()
        {
            sb.Clear();

            foreach (EnumMember em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryGetDisplayName))
                    continue;

                if (em.DisplayData?.Name == null)
                    continue;

                sb.Append('(').Append(sn).Append('.').Append(em.Name).Append(", \"").Append(em.DisplayData.Name).Append("\"),\n").Append(Indent(4));
            }

            return sb.ToString().TrimEnd(CodeConstants.TrimChars);
        }

        string GetDescriptions()
        {
            sb.Clear();

            foreach (EnumMember em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryGetDescription))
                    continue;

                if (em.DisplayData?.Description == null)
                    continue;

                sb.Append('(').Append(sn).Append('.').Append(em.Name).Append(", \"").Append(em.DisplayData.Description).Append("\"),\n").Append(Indent(4));
            }

            return sb.ToString().TrimEnd(CodeConstants.TrimChars);
        }

        string IsDefined()
        {
            if (es.Members.Count == 0)
                return "false";

            ulong value = 0;

            foreach (EnumMember em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.IsDefined))
                    continue;

                string strVal = em.Value.ToString();

                if (strVal.StartsWith("-", StringComparison.Ordinal))
                    value |= unchecked((ulong)long.Parse(strVal, NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
                else
                    value |= ulong.Parse(strVal, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }

            if (value == 0)
                return $"0 == ({ut})input";

            if (es.HasFlags)
                return $"unchecked((({ut}){value}UL & ({ut})input) == ({ut})input)";

            return $"Enum.IsDefined(typeof({sn}), input)";
        }

        res += "\n    }";

        if (!op.DisableEnumsWrapper)
            res += "\n}";

        return res;
    }
}