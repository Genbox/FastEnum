using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Genbox.FastEnum.Helpers;

internal static class TypeHelper
{
    public static T MapData<T>(ImmutableArray<KeyValuePair<string, TypedConstant>> data) where T : class, new()
    {
        T instance = new T();

        PropertyInfo[] props = typeof(T).GetProperties();

        Dictionary<string, PropertyInfo> indexed = new Dictionary<string, PropertyInfo>(props.Length, StringComparer.Ordinal);

        foreach (PropertyInfo info in props)
        {
            indexed.Add(info.Name, info);
        }

        foreach (KeyValuePair<string, TypedConstant> pair in data)
        {
            if (pair.Value.Value == null)
                continue;

            PropertyInfo prop = indexed[pair.Key];
            prop.SetValue(instance, pair.Value.Value);
        }

        return instance;
    }
}