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

        foreach (KeyValuePair<string, TypedConstant> pair in data)
        {
            // Data models may intentionally map only a subset of an attribute's properties.
            if (pair.Value.Value == null || !PropertyCache<T>.Properties.TryGetValue(pair.Key, out PropertyInfo? prop))
                continue;

            prop.SetValue(instance, pair.Value.Value);
        }

        return instance;
    }

    // Initialized once per data model; immutable after publication and safe for concurrent generators.
    private static class PropertyCache<T>
    {
        internal static readonly Dictionary<string, PropertyInfo> Properties = typeof(T).GetProperties()
                                                                                        .ToDictionary(property => property.Name, StringComparer.Ordinal);
    }
}