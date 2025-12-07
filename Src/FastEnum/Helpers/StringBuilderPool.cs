using System.Collections.Concurrent;

namespace Genbox.FastEnum.Helpers;

/// <summary>Simple pooled StringBuilder helper to reduce allocations in codegen.</summary>
internal static class StringBuilderPool
{
    private const int MaxRetainedCapacity = 16384;
    private const int MaxRetainedInstances = 64;

    private static readonly ConcurrentBag<StringBuilder> _pool = new ConcurrentBag<StringBuilder>();

    internal static StringBuilder Rent(int capacity = 4096)
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

        if (_pool.Count < MaxRetainedInstances && sb.Capacity < MaxRetainedCapacity)
        {
            sb.Clear();
            _pool.Add(sb);
        }

        return value;
    }
}