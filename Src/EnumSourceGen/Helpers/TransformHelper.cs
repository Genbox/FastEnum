using System.Text.RegularExpressions;
using Genbox.EnumSourceGen.Data;

namespace Genbox.EnumSourceGen.Helpers;

internal static class TransformHelper
{
    public static string TransformName(EnumSpec enumSpec, EnumMember enumMember)
    {
        //First we use the explicitly set override
        EnumTransformValueData? tranVal = enumMember.TransformValueData;

        if (tranVal != null && tranVal.ValueOverride != null)
            return tranVal.ValueOverride;

        //Then we fall back to using the EnumTransform (if set)
        if (enumSpec.TransformData != null)
            return TransformName(enumMember.Name, enumSpec.TransformData.Preset, enumSpec.TransformData.Regex, enumSpec.TransformData.CasePattern);

        return enumMember.Name;
    }

    public static string TransformName(string name, EnumTransform preset, string? regex, string? casePattern)
    {
        if (preset != EnumTransform.None)
        {
            return preset switch
            {
                EnumTransform.LowerCase => name.ToLowerInvariant(),
                EnumTransform.UpperCase => name.ToUpperInvariant(),
                _ => throw new ArgumentOutOfRangeException(nameof(preset))
            };
        }

        if (regex != null)
        {
            //Input:     [HelloWORLD]
            //Transform: [/WORLD/World/]
            //Output:    [HelloWorld]

            int idx = regex.IndexOf('/', 1);

            if (idx <= 0)
                throw new InvalidOperationException($"Invalid transform regex specified on {name}");

            string regexStr = regex.Substring(1, idx - 1);
            string replacementStr = regex.Substring(idx + 1, regex.Length - idx - 2);

            return Regex.Replace(name, regexStr, replacementStr, RegexOptions.None, TimeSpan.FromSeconds(1));
        }

        if (casePattern != null)
        {
            //Input:     [HelloWORLD]
            //Transform: [U_____llll]
            //Output:    [HelloWorld]

            char[] chars = new char[name.Length];

            int ptr = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (casePattern[i] == 'U')
                {
                    chars[ptr++] = char.ToUpperInvariant(name[i]);
                }
                else if (casePattern[i] == 'L')
                {
                    chars[ptr++] = char.ToLowerInvariant(name[i]);
                }
                else if (casePattern[i] == '_')
                {
                    chars[ptr++] = name[i];
                }
                else if (casePattern[i] == 'O')
                {
                    //do nothing
                }
            }

            return new string(chars, 0, ptr);
        }

        return name;
    }
}