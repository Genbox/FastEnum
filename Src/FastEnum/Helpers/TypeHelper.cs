using System.Collections.Immutable;
using System.Reflection;

namespace Genbox.FastEnum.Helpers;

internal static class TypeHelper
{
    public static T MapData<T>(ImmutableArray<KeyValuePair<string, TypedConstant>> data) where T : class, new()
    {
        T instance = new T();

        if (data.Length == 0)
            return instance;

        PropertyInfo[] props = typeof(T).GetProperties();

        Dictionary<string, PropertyInfo> indexed = new Dictionary<string, PropertyInfo>(props.Length, StringComparer.Ordinal);

        foreach (PropertyInfo info in props)
        {
            indexed.Add(info.Name, info);
        }

        foreach (KeyValuePair<string, TypedConstant> pair in data)
        {
            // Data models may intentionally map only a subset of an attribute's properties.
            if (pair.Value.Value == null || !indexed.TryGetValue(pair.Key, out PropertyInfo? prop))
                continue;

            prop.SetValue(instance, pair.Value.Value);
        }

        return instance;
    }
}