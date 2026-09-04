using System.Globalization;

namespace Genbox.FastEnum.Generators;

internal static class EnumClassCode
{
    internal static string Generate(EnumSpec spec, bool wrapperPublic, bool attributeWrapper)
    {
        FastEnumData options = spec.Data;
        string? namespaceName = options.EnumsClassNamespace ?? spec.Namespace;
        string className = spec.EmittedIdentifier;
        string wrapperName = options.EnumsClassName ?? "Enums";
        string enumName = spec.FullyQualifiedName;
        string inheritedVisibility = spec.IsPubliclyAccessible ? "public" : "internal";
        string visibility = options.EnumsClassVisibility == Visibility.Inherit ? inheritedVisibility : options.EnumsClassVisibility.ToString().ToLowerInvariant();
        string wrapperVisibility = wrapperPublic ? "public" : "internal";
        string underlyingType = spec.UnderlyingType;
        string enumFormat = namespaceName != null ? $"global::{namespaceName}.{spec.Name}Format" : $"global::{spec.Name}Format";
        EnumTransformData? transform = spec.TransformData;
        List<string> fields = new List<string>();

        string memberNames = Assignment("_names", "string", GetMemberNames());
        string memberValues = Assignment("_values", enumName, GetMemberValues());
        string underlyingValues = Assignment("_underlyingValues", underlyingType, GetUnderlyingValues());
        string tryParse = TryParse();
        string isDefined = IsDefined();
        string displayNames = MetadataMethod(spec.HasDisplay, "display names", "DisplayNames", "_displayNames", x => x.DisplayData?.Name, transform?.SortDisplayNames ?? EnumOrder.None, EnumOmitExclude.TryGetDisplayName);
        string descriptions = MetadataMethod(spec.HasDescription, "descriptions", "Descriptions", "_descriptions", x => x.DisplayData?.Description, transform?.SortDescriptions ?? EnumOrder.None, EnumOmitExclude.TryGetDescription);
        string generatedFields = fields.Count == 0 ? string.Empty : $"\n\n{string.Join("\n", fields.Select(x => $"{Indent(2)}{x}"))}\n";
        int memberCount = spec.Members.Count(x => x.OmitValueData?.Exclude != EnumOmitExclude.All);
        string wrapperAttribute = attributeWrapper ? $"{EnumGenerator.GeneratedCodeAttribute}\n" : string.Empty;
        string? wrapper = !options.DisableEnumsWrapper
            ? $"/// <summary>Contains generated helpers for <see cref=\"{enumName}\"/>.</summary>\n{wrapperAttribute}{wrapperVisibility} static partial class {wrapperName}\n{{"
            : null;

        return $$"""
                 {{(namespaceName != null ? $"\nnamespace {namespaceName};\n" : null)}}
                 {{wrapper}}
                     /// <summary>Provides generated helper methods for <see cref="{{enumName}}"/>.</summary>
                     {{EnumGenerator.GeneratedCodeAttribute}}
                     {{visibility}} static partial class {{className}}
                     {
                         /// <summary>Gets the number of enum members included in the generated APIs.</summary>
                         public const int MemberCount = {{memberCount.ToString(NumberFormatInfo.InvariantInfo)}};

                         /// <summary>Indicates whether <see cref="{{enumName}}"/> is a flags enum.</summary>
                         public const bool IsFlagEnum = {{spec.HasFlags.ToString().ToLowerInvariant()}};

                         /// <summary>Gets the generated names of the enum members.</summary>
                         /// <returns>An array containing the generated member names.</returns>
                         public static string[] GetMemberNames() => {{memberNames}}

                         /// <summary>Gets the enum member values.</summary>
                         /// <returns>An array containing the enum member values.</returns>
                         public static {{enumName}}[] GetMemberValues() => {{memberValues}}

                         /// <summary>Gets the underlying numeric values of the enum members.</summary>
                         /// <returns>An array containing the underlying values.</returns>
                         public static {{underlyingType}}[] GetUnderlyingValues() => {{underlyingValues}}

                         {{TryParseMethod("string", "string")}}

                 #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                         {{TryParseMethod("global::System.ReadOnlySpan<char>", "character span")}}

                         {{ParseMethod("global::System.ReadOnlySpan<char>", "character span", "{value.ToString()}")}}
                 #endif

                         {{ParseMethod("string", "string", "{value}")}}

                         /// <summary>Determines whether an enum value is defined by the generated metadata.</summary>
                         /// <param name="input">The enum value to test.</param>
                         /// <returns><see langword="true"/> if the value is defined; otherwise, <see langword="false"/>.</returns>
                         public static bool IsDefined({{enumName}} input)
                         {
                             {{isDefined}}
                         }{{displayNames}}{{descriptions}}{{generatedFields}}
                     }{{(!options.DisableEnumsWrapper ? "\n}" : null)}}
                 """;

        string TryParseMethod(string valueType, string label)
        {
            return IndentFollowingLines(
                $$"""
                /// <summary>Attempts to parse a {{label}} into an enum value.</summary>
                /// <param name="value">The {{label}} to parse.</param>
                /// <param name="result">When this method returns, contains the parsed enum value if parsing succeeded.</param>
                /// <param name="format">The formats to consider while parsing.</param>
                /// <param name="comparison">The string comparison to use.</param>
                /// <returns><see langword="true"/> if parsing succeeded; otherwise, <see langword="false"/>.</returns>
                public static bool TryParse({{valueType}} value, out {{enumName}} result, {{enumFormat}} format = {{enumFormat}}.Default, global::System.StringComparison comparison = global::System.StringComparison.Ordinal)
                {
                    {{tryParse}}
                    result = default;
                    return false;
                }
                """,
                2);
        }

        string ParseMethod(string valueType, string label, string errorValue)
        {
            return IndentFollowingLines(
                $$"""
                /// <summary>Parses a {{label}} into an enum value.</summary>
                /// <param name="value">The {{label}} to parse.</param>
                /// <param name="format">The formats to consider while parsing.</param>
                /// <param name="comparison">The string comparison to use.</param>
                /// <returns>The parsed enum value.</returns>
                /// <exception cref="global::System.ArgumentOutOfRangeException"><paramref name="value"/> does not represent a valid enum value.</exception>
                public static {{enumName}} Parse({{valueType}} value, {{enumFormat}} format = {{enumFormat}}.Default, global::System.StringComparison comparison = global::System.StringComparison.Ordinal)
                {
                    if (!TryParse(value, out {{enumName}} result, format, comparison))
                        throw new global::System.ArgumentOutOfRangeException($"Invalid value: {{errorValue}}");

                    return result;
                }
                """,
                2);
        }

        IEnumerable<string> GetMemberNames() => GetMembers(transform?.SortMemberNames ?? EnumOrder.None, EnumOmitExclude.GetMemberNames, m => TransformHelper.TransformName(spec, m), m => $"\"{EscapeString(TransformHelper.TransformName(spec, m))}\"");

        IEnumerable<string> GetMemberValues() => GetMembers(transform?.SortMemberValues ?? EnumOrder.None, EnumOmitExclude.GetMemberValues, ValueKey, m => $"{enumName}.{m.EmittedIdentifier}");

        IEnumerable<string> GetUnderlyingValues() => GetMembers(transform?.SortUnderlyingValues ?? EnumOrder.None, EnumOmitExclude.GetUnderlyingValues, ValueKey, m => FormatPrimitive(m.Value));

        IEnumerable<string> GetMembers(EnumOrder order, EnumOmitExclude exclusion, Func<EnumMemberSpec, IComparable> sortKey, Func<EnumMemberSpec, string> format)
        {
            return ApplySort(spec.Members, order, sortKey).Where(x => IsIncluded(x, exclusion)).Select(format);
        }

        string MetadataMethod(bool enabled, string label, string suffix, string field, Func<EnumMemberSpec, string?> getText, EnumOrder order, EnumOmitExclude exclusion) => !enabled ? string.Empty : $$"""


                                                /// <summary>Gets the {{label}} defined for the enum members.</summary>
                                                /// <returns>An array of enum values paired with their {{label}}.</returns>
                                                public static ({{enumName}}, string)[] Get{{suffix}}() => {{Assignment(field, $"({enumName}, string)", GetMetadataValues(getText, order, exclusion))}}
                                        """;

        IEnumerable<string> GetMetadataValues(Func<EnumMemberSpec, string?> getText, EnumOrder order, EnumOmitExclude exclusion)
        {
            IEnumerable<EnumMemberSpec> members = spec.Members.Where(x => getText(x) != null && x.OmitValueData?.Exclude.HasFlag(exclusion) != true);
            return ApplySort(members, order, x => getText(x)!)
                .Select(x => $"({enumName}.{x.EmittedIdentifier}, \"{EscapeString(getText(x)!)}\")");
        }

        string TryParse()
        {
            EnumMemberSpec[] members = spec.Members.Where(x => IsIncluded(x, EnumOmitExclude.TryParse)).ToArray();
            if (members.Length == 0)
                return string.Empty;

            return IndentFollowingLines(string.Join("\n", GetBlocks()), 1);

            IEnumerable<string> GetBlocks()
            {
                yield return ParseBlock("Name", m => TransformHelper.TransformName(spec, m));
                yield return ParseBlock("Value", m => FormatPrimitive(m.Value, false));

                if (spec.HasDisplay)
                    yield return ParseBlock("DisplayName", m => m.DisplayData?.Name);

                if (spec.HasDescription)
                    yield return ParseBlock("Description", m => m.DisplayData?.Description);
            }

            string ParseBlock(string format, Func<EnumMemberSpec, string?> getText)
            {
                string start = $$"""
                                 if ((format & {{enumFormat}}.{{format}}) == {{enumFormat}}.{{format}})
                                 {
                                 """;
                return $"{start}{IndentFollowingLines(string.Concat(GetChecks()), 1)}\n}}";

                IEnumerable<string> GetChecks()
                {
                    for (int i = 0; i < members.Length; i++)
                    {
                        string? text = getText(members[i]);
                        if (text != null)
                            yield return $"\n{ParseCheck(members[i], text)}";

                        if (i < members.Length - 1)
                            yield return "\n";
                    }
                }
            }

            string ParseCheck(EnumMemberSpec member, string text) => $$"""
                                                                       if (value.Equals("{{EscapeString(text)}}", comparison))
                                                                       {
                                                                           result = {{enumName}}.{{member.EmittedIdentifier}};
                                                                           return true;
                                                                       }
                                                                       """;
        }

        string IsDefined()
        {
            if (spec.HasFlags)
                return $"return {IsFlagDefined()};";

            bool canReuseUnderlyingValues = spec.Members.All(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetUnderlyingValues) != true) && spec.Members.All(x => x.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.IsDefined) != true);
            string[] values = spec.Members.Where(x => IsIncluded(x, EnumOmitExclude.IsDefined)).Select(x => FormatPrimitive(x.Value)).ToArray();

            if (!canReuseUnderlyingValues && values.Length == 0)
                return "return false;";

            string valuesExpression = canReuseUnderlyingValues
                ? $"{underlyingType}[] _isDefinedValues = GetUnderlyingValues();"
                : Assignment("_isDefinedValues", underlyingType, values);
            string separator = canReuseUnderlyingValues ? "\n" : string.Empty;

            return $$"""
                     {{valuesExpression}}{{separator}}
                                 for (int i = 0; i < _isDefinedValues.Length; i++)
                                 {
                                     if (_isDefinedValues[i] == ({{underlyingType}})input)
                                         return true;
                                 }

                                 return false;
                     """;
        }

        string IsFlagDefined()
        {
            EnumMemberSpec[] includedMembers = spec.Members.Where(x => IsIncluded(x, EnumOmitExclude.IsDefined)).ToArray();
            if (includedMembers.Length == 0)
                return "false";

            HashSet<object> handledValues = new HashSet<object>(includedMembers.Select(x => x.Value));
            string exclusions = string.Concat(spec.Members.Where(x => !IsIncluded(x, EnumOmitExclude.IsDefined) && handledValues.Add(x.Value))
                                                  .Select(x => $"input != {enumName}.{x.EmittedIdentifier} && "));
            ulong mask = includedMembers.Aggregate(0UL, (value, member) => value | ToUInt64(member.Value));
            string maskCheck = mask == 0
                ? $"({underlyingType})input == 0"
                : $"unchecked((({underlyingType}){mask}UL & ({underlyingType})input) == ({underlyingType})input)";
            return exclusions + maskCheck;
        }

        string Assignment(string name, string type, IEnumerable<string> elements)
        {
            string[] values = elements.ToArray();
            if (values.Length == 0)
                return $"global::System.Array.Empty<{type}>();";

            string assignment;

            if (options.DisableCache)
                assignment = $"new {type}[]";
            else
            {
                fields.Add($"private static {type}[]? {name};");
                assignment = $"{name} ??= new {type}[]";
            }

            return $$"""
                     {{assignment}} {
                                     {{IndentFollowingLines(string.Join(",\n", values), 4)}}
                                 };
                     """;
        }

        static bool IsIncluded(EnumMemberSpec member, EnumOmitExclude exclusion) => member.OmitValueData?.Exclude.HasFlag(exclusion) != true;

        static string IndentFollowingLines(string value, int amount)
        {
            string indent = Indent(amount);
            string[] lines = value.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length > 0)
                    lines[i] = $"{indent}{lines[i]}";
            }

            return string.Join("\n", lines);
        }

        static IEnumerable<EnumMemberSpec> ApplySort(IEnumerable<EnumMemberSpec> members, EnumOrder order, Func<EnumMemberSpec, IComparable> selector) => order switch
        {
            EnumOrder.Ascending => members.OrderBy(selector),
            EnumOrder.Descending => members.OrderByDescending(selector),
            _ => members
        };

        static IComparable ValueKey(EnumMemberSpec member) => (IComparable)member.Value;
    }
}