using System.Globalization;

namespace Genbox.FastEnum.Generators;

internal static class EnumLookupCode
{
    internal const int HashThreshold = 32;

    internal static bool UsesSwitch(EnumSpec spec, int entryCount) => spec.Data.DisableCache || entryCount < HashThreshold;

    // Callers filter omissions and resolve aliases before constructing the lookup.
    internal static string Create(EnumSpec spec, EnumMemberSpec[] members, string name, string input,
                                  Func<EnumMemberSpec, string>? result, List<string> fields, string? fallback = null)
    {
        string type = spec.UnderlyingType;
        string returnType = result == null ? "bool" : "string?";
        string missing = fallback ?? (result == null ? "false" : "null");
        if (members.Length == 0)
            return missing;

        if (UsesSwitch(spec, members.Length))
        {
            string arms = string.Join("\n", members.Select(x => $"    {FormatPrimitive(x.Value)} => {(result == null ? "true" : result(x))},"));
            return $$"""
                     ((({{type}}){{input}}) switch
                     {
                     {{arms}}
                         _ => {{missing}}
                     })
                     """;
        }

        // Extension classes may be shared by enums from different namespaces.
        // Escape punctuation (including underscores) so the suffix is collision-free.
        name += "_" + string.Concat(spec.FullyQualifiedName.Select(c =>
            c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9'
                ? c.ToString()
                : "_" + ((int)c).ToString("X4", CultureInfo.InvariantCulture)));

        List<string> arrays = new List<string>();
        if (TryGetDenseMinimum(members, out ulong minimum))
            return CreateDenseLookup(minimum);

        AddArray(type, "_values", members.Select(x => FormatPrimitive(x.Value)));
        if (result != null)
            AddArray("string", "_results", members.Select(result));

        string found = result == null ? "true" : "_results[index]";
        EnumHashTable table = EnumHashTable.Create(members.Select(x => ToUInt64(x.Value)).ToArray());
        AddArray("int", "_buckets", table.Buckets.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        AddArray("int", "_next", table.Next.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        string key = table.Shift == 0 ? "unchecked((int)input)" : $"unchecked((int)((ulong)input >> {table.Shift}))";

        // A nested holder initializes only this lookup and never exposes its arrays.
        string declarations = string.Join("\n", arrays);
        fields.Add($$"""
                     private static class {{name}}
                     {
                         {{IndentFollowingLines(declarations, 1)}}

                         [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                         internal static {{returnType}} Find({{type}} input)
                         {
                             int bucket = {{key}} & {{table.Buckets.Length - 1}};
                             for (int index = _buckets[bucket]; index >= 0; index = _next[index])
                             {
                                 if (_values[index] == input)
                                     return {{found}};
                             }
                             return {{missing}};
                         }
                     }
                     """);
        return $"{name}.Find(({type}){input})";

        string CreateDenseLookup(ulong minimumValue)
        {
            bool requires64BitIndex = members[0].Value is long or ulong;
            string indexType = requires64BitIndex ? "ulong" : "uint";
            string literalSuffix = requires64BitIndex ? "UL" : "U";
            ulong indexBase = requires64BitIndex ? minimumValue : unchecked((uint)minimumValue);
            string minimumLiteral = indexBase.ToString(CultureInfo.InvariantCulture) + literalSuffix;
            string countLiteral = members.Length.ToString(CultureInfo.InvariantCulture) + literalSuffix;

            // Unsigned subtraction wraps deliberately, including for signed ranges crossing zero.
            // Membership needs only a bounds check; text lookups also need an ordered result array.
            if (result == null)
            {
                string offset = $"unchecked(({indexType})({type}){input} - {minimumLiteral})";
                return $"({offset} < {countLiteral})";
            }

            IEnumerable<string> orderedResults = members.OrderBy(member => (IComparable)member.Value).Select(result);
            AddArray("string", "_results", orderedResults);
            fields.Add($$"""
                         private static class {{name}}
                         {
                             {{IndentFollowingLines(string.Join("\n", arrays), 1)}}

                             [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                             internal static string? Find({{type}} input)
                             {
                                 {{indexType}} index = unchecked(({{indexType}})input - {{minimumLiteral}});
                                 return index < ({{indexType}})_results.Length ? _results[(int)index] : {{missing}};
                             }
                         }
                         """);
            return $"{name}.Find(({type}){input})";
        }

        void AddArray(string elementType, string arrayName, IEnumerable<string> entries) => arrays.Add($$"""private static readonly {{elementType}}[] {{arrayName}} = new {{elementType}}[] { {{string.Join(", ", entries)}} };""");
    }

    private static bool TryGetDenseMinimum(EnumMemberSpec[] members, out ulong minimum)
    {
        object minimumValue = members[0].Value;
        object maximumValue = minimumValue;

        foreach (EnumMemberSpec member in members)
        {
            IComparable value = (IComparable)member.Value;
            if (value.CompareTo(minimumValue) < 0)
                minimumValue = member.Value;
            if (value.CompareTo(maximumValue) > 0)
                maximumValue = member.Value;
        }

        minimum = ToUInt64(minimumValue);

        // Members are unique. A range with exactly this many slots cannot contain holes.
        return unchecked(ToUInt64(maximumValue) - minimum) == (ulong)(members.Length - 1);
    }
}