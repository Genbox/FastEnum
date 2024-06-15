using System.Text.RegularExpressions;
using Genbox.FastEnum.Data;
using Genbox.FastEnum.Spec;

namespace Genbox.FastEnum.Helpers;

internal static class TransformHelper
{
    public static string TransformName(EnumSpec enumSpec, EnumMemberSpec enumMember)
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

            //Input:     [HelloWorld]
            //Transform: [LOO]
            //Output:    [hloWorld]

            char[] chars = new char[name.Length];

            //We iterate
            int length = Math.Min(name.Length, casePattern.Length);

            int charPtr = 0;
            int namePtr = 0;

            for (; namePtr < length; namePtr++)
            {
                if (casePattern[namePtr] == 'U')
                {
                    chars[charPtr++] = char.ToUpperInvariant(name[namePtr]);
                }
                else if (casePattern[namePtr] == 'L')
                {
                    chars[charPtr++] = char.ToLowerInvariant(name[namePtr]);
                }
                else if (casePattern[namePtr] == '_')
                {
                    chars[charPtr++] = name[namePtr];
                }
                else if (casePattern[namePtr] == 'O')
                {
                    //do nothing
                }
            }

            //If there is anything left of name that we have not copied, copy it now.
            for (; namePtr < name.Length; namePtr++)
            {
                chars[charPtr++] = name[namePtr];
            }

            return new string(chars, 0, charPtr);
        }

        return name;
    }
}