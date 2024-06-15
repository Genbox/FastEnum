using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Genbox.FastEnum.Benchmarks.Code;

internal static class EnumHelper<T> where T : struct
{
    internal static bool TryParseByDisplayName(string name, bool ignoreCase, out T enumValue)
    {
        enumValue = default;

        StringComparison comparer = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        T[] enumValues = (T[])Enum.GetValues(typeof(T));

        foreach (T value in enumValues)
        {
            if (TryGetDisplayName(value.ToString(), out string? displayName) && displayName.Equals(name, comparer))
            {
                enumValue = value;
                return true;
            }
        }

        return false;
    }

    private static bool TryGetDisplayName(string? value, [NotNullWhen(true)]out string? displayName)
    {
        displayName = default;

        if (!typeof(T).IsEnum)
            return false;

        if (value is null)
            return false;

        MemberInfo[] memberInfo = typeof(T).GetMember(value);
        if (memberInfo.Length <= 0)
            return false;

        displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
        return displayName != null;
    }

    internal static string GetDisplayName(T value)
    {
        if (!typeof(T).IsEnum)
            return string.Empty;

        MemberInfo[] memberInfo = typeof(T).GetMember(value.ToString()!);
        if (memberInfo.Length <= 0)
            return string.Empty;

        string? displayName = memberInfo[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
        return displayName ?? string.Empty;
    }
}