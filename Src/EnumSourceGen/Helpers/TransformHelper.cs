using System.Text.RegularExpressions;
using Genbox.EnumSourceGen.Data;

namespace Genbox.EnumSourceGen.Helpers;

internal static class TransformHelper
{
    public static string TransformName(EnumMember enumMember)
    {
        EnumTransformValueData? tranVal = enumMember.TransformValueData;

        if (tranVal == null)
            return enumMember.Name;

        return TransformName(enumMember.Name, tranVal.NameOverride, tranVal.TransformPreset, tranVal.Transform);
    }

    public static string TransformName(string name, string? nameOverride, EnumTransform preset, string? transform)
    {
        if (nameOverride != null)
            return nameOverride;

        if (preset != EnumTransform.None)
        {
            return preset switch
            {
                EnumTransform.LowerCase => name.ToLowerInvariant(),
                EnumTransform.UpperCase => name.ToUpperInvariant(),
                _ => throw new ArgumentOutOfRangeException(nameof(preset))
            };
        }

        if (transform != null)
        {
            //Input:     [HelloWORLD]
            //Transform: [/WORLD/World/]
            //Output:    [HelloWorld]
            if (transform.StartsWith("/", StringComparison.Ordinal)) //Regex mode
            {
                int idx = transform.IndexOf('/', 1);

                if (idx <= 0)
                    throw new InvalidOperationException($"Invalid transform regex specified on {name}");

                string regexStr = transform.Substring(1, idx - 1);
                string replacementStr = transform.Substring(idx + 1, transform.Length - idx - 2);

                return Regex.Replace(name, regexStr, replacementStr, RegexOptions.None, TimeSpan.FromSeconds(1));
            }

            //Input:     [HelloWORLD]
            //Transform: [U_____llll]
            //Output:    [HelloWorld]

            if (name.Length != transform.Length)
                throw new InvalidOperationException($"The length of your AdvancedTransform template must be as long as the enum value. AdvancedTransform: {transform} ({transform.Length}) Enum value: {name} ({name.Length})");

            char[] chars = new char[name.Length];

            int ptr = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (transform[i] == 'U')
                {
                    chars[ptr++] = char.ToUpperInvariant(name[i]);
                }
                else if (transform[i] == 'L')
                {
                    chars[ptr++] = char.ToLowerInvariant(name[i]);
                }
                else if (transform[i] == '_')
                {
                    chars[ptr++] = name[i];
                }
                else if (transform[i] == 'O')
                {
                    //do nothing
                }
            }

            return new string(chars, 0, ptr);
        }

        return name;
    }
}