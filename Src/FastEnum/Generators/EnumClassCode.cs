using System.Globalization;

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
        int mc = es.Members.Count(x => x.OmitValueData?.Exclude != EnumOmitExclude.All);
        bool omitUnderlyingValues = es.Members.Any(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetUnderlyingValues) == true);
        bool omitIsDefined = es.Members.Any(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.IsDefined) == true);
        string ef = (ns != null ? ns + '.' : null) + cn + "Format";
        EnumTransformData? transform = es.TransformData;

        List<string> fields = new List<string>();

        StringBuilder sb = StringBuilderPool.Rent(4096);

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
                $"""


                         public static ({sn}, string)[] GetDisplayNames() => {Assignment("_displayNames", $"({sn}, string)", op.DisableCache, fields, GetDisplayNames())}
                 """;
        }

        if (es.HasDescription)
        {
            res +=
                $"""


                         public static ({sn}, string)[] GetDescriptions() => {Assignment("_descriptions", $"({sn}, string)", op.DisableCache, fields, GetDescriptions())}
                 """;
        }

        if (fields.Count > 0)
        {
            sb.Clear();
            sb.AppendLine();
            sb.AppendLine();

            foreach (string field in fields)
            {
                sb.Append(Indent(2)).AppendLine(field);
            }

            res += StringBuilderPool.ReturnGetString(sb);
        }

        res += "\n    }";

        if (!op.DisableEnumsWrapper)
            res += "\n}";

        return res;

        IEnumerable<string> GetMemberNames()
        {
            foreach (EnumMemberSpec em in ApplySort(es.Members, transform?.SortMemberNames ?? EnumOrder.None, m => TransformHelper.TransformName(es, m)))
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetMemberNames) == true)
                    continue;

                yield return $"\"{EscapeString(TransformHelper.TransformName(es, em))}\"";
            }
        }

        IEnumerable<string> GetMemberValues()
        {
            foreach (EnumMemberSpec em in ApplySort(es.Members, transform?.SortMemberValues ?? EnumOrder.None, ValueKey))
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetMemberValues) == true)
                    continue;

                yield return $"{sn}.{em.Name}";
            }
        }

        IEnumerable<string> GetUnderlyingValues()
        {
            foreach (EnumMemberSpec em in ApplySort(es.Members, transform?.SortUnderlyingValues ?? EnumOrder.None, ValueKey))
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetUnderlyingValues) == true)
                    continue;

                yield return FormatPrimitive(em.Value);
            }
        }

        IEnumerable<string> GetDisplayNames()
        {
            IEnumerable<EnumMemberSpec> filtered = es.Members.Where(x => x.DisplayData?.Name != null && x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetDisplayName) != true);

            foreach (EnumMemberSpec em in ApplySort(filtered, transform?.SortDisplayNames ?? EnumOrder.None, DisplayNameKey))
                yield return $"({sn}.{em.Name}, \"{EscapeString(em.DisplayData.Name)}\")";
        }

        IEnumerable<string> GetDescriptions()
        {
            IEnumerable<EnumMemberSpec> filtered = es.Members.Where(x => x.DisplayData?.Description != null && x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetDescription) != true);

            foreach (EnumMemberSpec em in ApplySort(filtered, transform?.SortDescriptions ?? EnumOrder.None, DescriptionKey))
                yield return $"({sn}.{em.Name}, \"{EscapeString(em.DisplayData.Description)}\")";
        }

        IEnumerable<EnumMemberSpec> GetTryParseMembers()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryParse) == true)
                    continue;

                yield return em;
            }
        }

        IEnumerable<string> IsDefinedMembers()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.IsDefined) == true)
                    continue;

                yield return FormatPrimitive(em.Value);
            }
        }

        string TryParse()
        {
            EnumMemberSpec[] members = GetTryParseMembers().ToArray();

            if (members.Length == 0)
                return string.Empty;

            sb.Clear();
            sb.Append($$"""
                        if ((format & {{ef}}.Name) == {{ef}}.Name)
                                    {
                        """);

            for (int i = 0; i < members.Length; i++)
            {
                EnumMemberSpec em = members[i];

                sb.Append($$"""

                                            if (value.Equals("{{EscapeString(TransformHelper.TransformName(es, em))}}", comparison))
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

                                    if ((format & {{ef}}.Value) == {{ef}}.Value)
                                    {
                        """);

            for (int i = 0; i < members.Length; i++)
            {
                EnumMemberSpec em = members[i];

                string escapedValue = EscapeString(FormatPrimitive(em.Value, false));

                sb.Append($$"""

                                            if (value.Equals("{{escapedValue}}", comparison))
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

                                        if ((format & {{ef}}.DisplayName) == {{ef}}.DisplayName)
                                        {
                            """);

                for (int i = 0; i < members.Length; i++)
                {
                    EnumMemberSpec em = members[i];

                    if (em.DisplayData?.Name != null)
                    {
                        string escapedDisplayName = EscapeString(em.DisplayData.Name);

                        sb.Append($$"""

                                                    if (value.Equals("{{escapedDisplayName}}", comparison))
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

                                        if ((format & {{ef}}.Description) == {{ef}}.Description)
                                        {
                            """);

                for (int i = 0; i < members.Length; i++)
                {
                    EnumMemberSpec em = members[i];

                    if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryParse) == true)
                        continue;

                    if (em.DisplayData?.Description != null)
                    {
                        string escapedDisplayDesc = EscapeString(em.DisplayData.Description);
                        sb.Append($$"""

                                                    if (value.Equals("{{escapedDisplayDesc}}", comparison))
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

            //If we have no omissions impacting IsDefined, then we can reuse GetUnderlyingValues()
            if (!omitUnderlyingValues && !omitIsDefined)
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
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.IsDefined) == true)
                    continue;

                value |= ToUInt64(em.Value);
            }

            if (value == 0)
                return $"({ut})input == 0";

            return $"unchecked((({ut}){value}UL & ({ut})input) == ({ut})input)";
        }
    }

    private static ulong ToUInt64(object value) => value switch
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

    private static IEnumerable<EnumMemberSpec> ApplySort(IEnumerable<EnumMemberSpec> members, EnumOrder order, Func<EnumMemberSpec, IComparable> selector)
    {
        return order switch
        {
            EnumOrder.Ascending => members.OrderBy(selector),
            EnumOrder.Descending => members.OrderByDescending(selector),
            _ => members
        };
    }

    private static IComparable ValueKey(EnumMemberSpec em) => (IComparable)em.Value;

    private static IComparable DisplayNameKey(EnumMemberSpec em) => em.DisplayData!.Name!;

    private static IComparable DescriptionKey(EnumMemberSpec em) => em.DisplayData!.Description!;

    private static string Assignment(string name, string type, bool cacheDisabled, List<string> fields, IEnumerable<string> elements)
    {
        string[] arr = elements.ToArray();

        if (arr.Length == 0)
            return $"Array.Empty<{type}>();";

        StringBuilder sb = StringBuilderPool.Rent(100);

        if (cacheDisabled)
            sb.Append("new ").Append(type).AppendLine("[] {");
        else
        {
            fields.Add($"private static {type}[]? {name};");
            sb.Append(name).Append(" ??= new ").Append(type).Append("[] {\n");
        }

        for (int i = 0; i < arr.Length; i++)
        {
            sb.Append(Indent(4)).Append(arr[i]);

            if (i != arr.Length - 1)
                sb.Append(',');

            sb.Append('\n');
        }

        sb.Append(Indent(3)).Append("};");

        return StringBuilderPool.ReturnGetString(sb);
    }
}