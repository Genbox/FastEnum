using System.Globalization;
using System.Text;
using Genbox.FastEnum.Data;
using Genbox.FastEnum.Helpers;
using Genbox.FastEnum.Spec;
using Microsoft.CodeAnalysis;

namespace Genbox.FastEnum.Generators;

internal static class EnumClassCode
{
    internal static string Generate(EnumSpec es)
    {
        FastEnumData op = es.Data;

        string? ns = op.EnumsClassNamespace ?? es.Namespace;
        string cn = op.EnumNameOverride ?? es.Name;
        string en = op.EnumsClassName ?? "Enums";
        string sn = es.Namespace == null ? "global::" + es.FullyQualifiedName : es.FullyQualifiedName;
        string vi = op.EnumsClassVisibility == Visibility.Inherit ? (es.AccessChain[0] == Accessibility.Public ? "public" : "internal") : op.EnumsClassVisibility.ToString().ToLowerInvariant();
        string ut = es.UnderlyingType;
        int oc = es.Members.Count(x => x.OmitValueData != null);
        int mc = es.Members.Length - oc;
        string ef = (ns != null ? ns + '.' : null) + cn + "Format";

        List<string> fields = new List<string>();

        StringBuilder sb = new StringBuilder();

        string res = $$"""
using System;
{{(ns != null ? "\nnamespace " + ns + ";\n" : null)}}
{{(!op.DisableEnumsWrapper ? $"{vi} static partial class {en}\n{{" : "")}}
    {{vi}} static partial class {{cn}}
    {
        public const int MemberCount = {{mc.ToString(NumberFormatInfo.InvariantInfo)}};
        public const bool IsFlagEnum = {{es.HasFlags.ToString().ToLowerInvariant()}};

        public static string[] GetMemberNames() => {{Assignment("_names", "string", op.DisableCache, fields, GetMemberNames())}}

        public static {{sn}}[] GetMemberValues() => {{Assignment("_values", sn, op.DisableCache, fields, GetMemberValues())}}

        public static {{ut}}[] GetUnderlyingValues() => {{Assignment("_underlyingValues", ut, op.DisableCache, fields, GetUnderlyingValues())}}

        public static bool TryParse(string value, out {{sn}} result, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
            {{TryParse()}}
            result = default;
            return false;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        public static bool TryParse(ReadOnlySpan<char> value, out {{sn}} result, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
            {{TryParse()}}
            result = default;
            return false;
        }

        public static {{sn}} Parse(ReadOnlySpan<char> value, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
            if (!TryParse(value, out {{sn}} result, format, comparison))
                throw new ArgumentOutOfRangeException($"Invalid value: {value.ToString()}");

            return result;
        }
#endif

        public static {{sn}} Parse(string value, {{ef}} format = {{ef}}.Default, StringComparison comparison = StringComparison.Ordinal)
        {
            if (!TryParse(value, out {{sn}} result, format, comparison))
                throw new ArgumentOutOfRangeException($"Invalid value: {value}");

            return result;
        }

        public static bool IsDefined({{sn}} input)
        {
            {{IsDefined()}}
        }
""";

        if (es.HasDisplay)
        {
            res +=
                $$"""


        public static ({{sn}}, string)[] GetDisplayNames() => {{Assignment("_displayNames", $"({sn}, string)", op.DisableCache, fields, GetDisplayNames())}}
""";
        }

        if (es.HasDescription)
        {
            res +=
                $$"""


        public static ({{sn}}, string)[] GetDescriptions() => {{Assignment("_descriptions", $"({sn}, string)", op.DisableCache, fields, GetDescriptions())}}
""";
        }

        IEnumerable<string> GetMemberNames()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.GetMemberNames))
                    continue;

                yield return $"\"{TransformHelper.TransformName(es, em)}\"";
            }
        }

        IEnumerable<string> GetMemberValues()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.GetMemberValues))
                    continue;

                yield return $"{sn}.{em.Name}";
            }
        }

        IEnumerable<string> GetUnderlyingValues()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.GetUnderlyingValues))
                    continue;

                yield return em.Value.ToString();
            }
        }

        IEnumerable<string> GetDisplayNames()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryGetDisplayName))
                    continue;

                if (em.DisplayData?.Name == null)
                    continue;

                yield return $"({sn}.{em.Name}, \"{em.DisplayData.Name}\")";
            }
        }

        IEnumerable<string> GetDescriptions()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryGetDescription))
                    continue;

                if (em.DisplayData?.Description == null)
                    continue;

                yield return $"({sn}.{em.Name}, \"{em.DisplayData.Description}\")";
            }
        }

        IEnumerable<EnumMemberSpec> GetTryParseMembers()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.TryParse))
                    continue;

                yield return em;
            }
        }

        IEnumerable<string> IsDefinedMembers()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData != null && !em.OmitValueData.Exclude.HasFlag(EnumOmitExclude.IsDefined))
                    continue;

                yield return em.Value.ToString();
            }
        }

        string TryParse()
        {
            EnumMemberSpec[] members = GetTryParseMembers().ToArray();

            if (members.Length == 0)
                return string.Empty;

            sb.Clear();
            sb.Append($$"""
if (format.HasFlag({{ef}}.Name))
            {
""");

            for (int i = 0; i < members.Length; i++)
            {
                EnumMemberSpec em = members[i];

                sb.Append($$"""

                if (value.Equals("{{em.Name}}", comparison))
                {
                    result = {{sn}}.{{em.Name}};
                    return true;
                }
""");

                if (i != members.Length - 1)
                    sb.AppendLine();
            }

            sb.Append("\n            }");

            sb.Append($$"""

            if (format.HasFlag({{ef}}.Value))
            {
""");

            for (int i = 0; i < members.Length; i++)
            {
                EnumMemberSpec em = members[i];

                sb.Append($$"""

                if (value.Equals("{{em.Value}}", comparison))
                {
                    result = {{sn}}.{{em.Name}};
                    return true;
                }
""");

                if (i != members.Length - 1)
                    sb.AppendLine();
            }

            sb.Append("\n            }");

            if (es.HasDisplay)
            {
                sb.Append($$"""

            if (format.HasFlag({{ef}}.DisplayName))
            {
""");

                for (int i = 0; i < members.Length; i++)
                {
                    EnumMemberSpec em = members[i];

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
                    if (i != members.Length - 1)
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

                for (int i = 0; i < members.Length; i++)
                {
                    EnumMemberSpec em = members[i];

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

                    if (i != members.Length - 1)
                        sb.AppendLine();
                }

                sb.Append("\n            }");
            }

            return sb.ToString();
        }

        string IsDefined()
        {
            if (es.HasFlags)
                return $"return {IsFlagDefined()};";

            sb.Clear();

            bool hasMembers = true;

            //If we have no omitted enum values, then we can reuse GetUnderlyingValues()
            if (oc == 0)
                sb.Append(ut).AppendLine("[] _isDefinedValues = GetUnderlyingValues();");
            else
            {
                string[] arr = IsDefinedMembers().ToArray();
                string assignment = Assignment("_isDefinedValues", ut, op.DisableCache, fields, arr);

                hasMembers = arr.Length > 0;

                if (!hasMembers)
                    sb.Append("return false;");
                else
                    sb.Append(assignment);
            }

            if (hasMembers)
            {
                sb.Append($$"""

            for (int i = 0; i < _isDefinedValues.Length; i++)
            {
                if (_isDefinedValues[i] == ({{ut}})input)
                    return true;
            }

            return false;
""");
            }

            return sb.ToString();
        }

        string IsFlagDefined()
        {
            if (es.Members.Length == 0)
                return "false";

            ulong value = 0;

            foreach (EnumMemberSpec em in es.Members)
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
                return $"({ut})input == 0";

            return $"unchecked((({ut}){value}UL & ({ut})input) == ({ut})input)";
        }

        if (fields.Count > 0)
        {
            sb.Clear();
            sb.AppendLine();
            sb.AppendLine();

            foreach (string field in fields)
            {
                sb.Append(CodeGenHelper.Indent(2)).AppendLine(field);
            }

            res += sb.ToString();
        }

        res += "\n    }";

        if (!op.DisableEnumsWrapper)
            res += "\n}";

        return res;
    }

    private static string Assignment(string name, string type, bool cacheDisabled, List<string> fields, IEnumerable<string> elements)
    {
        string[] arr = elements.ToArray();

        if (arr.Length == 0)
            return $"Array.Empty<{type}>();";

        StringBuilder sb = new StringBuilder(100);

        if (cacheDisabled)
            sb.Append(type).Append("[] ").Append(name).AppendLine(" = {");
        else
        {
            fields.Add($"private static {type}[]? {name};");
            sb.Append(name).Append(" ??= new ").Append(type).Append("[] {\n");
        }

        for (int i = 0; i < arr.Length; i++)
        {
            sb.Append(CodeGenHelper.Indent(4)).Append(arr[i]);

            if (i != arr.Length - 1)
                sb.Append(',');

            sb.Append('\n');
        }

        sb.Append(CodeGenHelper.Indent(3)).Append("};");

        return sb.ToString();
    }
}