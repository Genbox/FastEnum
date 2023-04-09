using System.Text.RegularExpressions;

namespace Genbox.EnumSourceGen.Helpers;

internal static class TransformHelper
{
    public static string TransformName(string name, string? nameOverride, EnumTransform? simpleTransform, string? advancedTransform)
    {
        if (nameOverride != null)
            return nameOverride;

        if (simpleTransform != null)
        {
            return simpleTransform switch
            {
                EnumTransform.LowerCase => name.ToLowerInvariant(),
                EnumTransform.UpperCase => name.ToUpperInvariant(),
                _ => throw new ArgumentOutOfRangeException(nameof(simpleTransform))
            };
        }

        if (advancedTransform != null)
        {
            //Input:     [HelloWORLD]
            //Transform: [/WORLD/World/]
            //Output:    [HelloWorld]
            if (advancedTransform.StartsWith("/", StringComparison.Ordinal)) //Regex mode
            {
                int idx = advancedTransform.IndexOf('/', 1);

                if (idx <= 0)
                    throw new InvalidOperationException($"Invalid transform regex specified on {name}");

                string regexStr = advancedTransform.Substring(1, idx - 1);
                string replacementStr = advancedTransform.Substring(idx + 1, advancedTransform.Length - idx - 2);

                return Regex.Replace(name, regexStr, replacementStr, RegexOptions.None, TimeSpan.FromSeconds(1));
            }

            //Input:     [HelloWORLD]
            //Transform: [U_____llll]
            //Output:    [HelloWorld]

            if (name.Length != advancedTransform.Length)
                throw new InvalidOperationException($"The length of your AdvancedTransform template must be as long as the enum value. AdvancedTransform: {advancedTransform} ({advancedTransform.Length}) Enum value: {name} ({name.Length})");

            char[] chars = new char[name.Length];

            int ptr = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (advancedTransform[i] == 'U')
                {
                    chars[ptr++] = char.ToUpperInvariant(name[i]);
                }
                else if (advancedTransform[i] == 'L')
                {
                    chars[ptr++] = char.ToLowerInvariant(name[i]);
                }
                else if (advancedTransform[i] == '_')
                {
                    chars[ptr++] = name[i];
                }
                else if (advancedTransform[i] == 'O')
                {
                    //do nothing
                }
            }

            return new string(chars, 0, ptr);
        }

        return name;
    }
}