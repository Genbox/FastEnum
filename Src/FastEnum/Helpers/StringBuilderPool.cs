using System.Collections.Concurrent;

namespace Genbox.FastEnum.Helpers;

/// <summary>Simple pooled StringBuilder helper to reduce allocations in codegen.</summary>
internal static class StringBuilderPool
{
    private const int MaxRetainedCapacity = 4096;
    private const int MaxRetainedInstances = 8;

    private static readonly ConcurrentBag<StringBuilder> _pool = new ConcurrentBag<StringBuilder>();

    internal static StringBuilder Rent(int capacity = 16)
    {
        if (!_pool.TryTake(out StringBuilder? sb))
            return new StringBuilder(capacity);

        sb.Clear();

        if (sb.Capacity < capacity)
            sb.EnsureCapacity(capacity);

        return sb;
    }

    internal static string ReturnGetString(StringBuilder sb)
    {
        string value = sb.ToString();

        if (sb.Capacity > MaxRetainedCapacity)
            sb.Capacity = MaxRetainedCapacity;

        sb.Clear();

        if (_pool.Count < MaxRetainedInstances)
            _pool.Add(sb);

        return value;
    }
}