namespace Genbox.FastEnum.Generators;

internal static class EnumParseCode
{
    // ASCII keys can be partitioned without changing ordinal comparison semantics.
    // Leaves still compare the whole input, preserving aliases and case sensitivity.
    internal static string Create((EnumMemberSpec Member, string Text)[] entries, Func<EnumMemberSpec, string, string> check, Func<string, string> extractMethod)
    {
        string cases = string.Join("\n", entries.GroupBy(entry => entry.Text.Length).Select(group => $$"""
                                                                                                       case {{group.Key}}:
                                                                                                           {{IndentFollowingLines(Branch(group.ToArray()), 1)}}
                                                                                                       """));
        return $$"""
                 switch (value.Length)
                 {
                     {{IndentFollowingLines(cases, 1)}}
                     default:
                         return false;
                 }
                 """;

        string Branch((EnumMemberSpec Member, string Text)[] candidates)
        {
            string body = Partition(candidates);

            // Bound method size so large span parsers remain eligible for JIT optimization.
            return entries.Length > 32 && candidates.Length <= 32 ? extractMethod(body) : body;
        }

        string Partition((EnumMemberSpec Member, string Text)[] candidates)
        {
            int bestPosition = -1;
            int bestGroups = 1;

            for (int position = 0; position < candidates[0].Text.Length; position++)
            {
                int groups = candidates.Select(entry => char.ToUpperInvariant(entry.Text[position])).Distinct().Count();

                if (groups > bestGroups)
                {
                    bestGroups = groups;
                    bestPosition = position;
                }
            }

            if (bestPosition < 0)
                return string.Join("\n", candidates.Select(entry => check(entry.Member, entry.Text))) + "\nreturn false;";

            string branches = string.Join("\n", candidates.GroupBy(entry => char.ToUpperInvariant(entry.Text[bestPosition])).Select(group => $$"""
                                                                                                                                               case {{(int)group.Key}}:
                                                                                                                                                   {{IndentFollowingLines(candidates.Length > 32 ? Branch(group.ToArray()) : Partition(group.ToArray()), 1)}}
                                                                                                                                               """));
            bool ignoreCase = candidates.Any(entry => char.IsLetter(entry.Text[bestPosition]));
            string discriminator = ignoreCase ? $"char.ToUpperInvariant(value[{bestPosition}])" : $"value[{bestPosition}]";
            return $$"""
                     switch ((int){{discriminator}})
                     {
                         {{IndentFollowingLines(branches, 1)}}
                         default:
                             return false;
                     }
                     """;
        }
    }
}