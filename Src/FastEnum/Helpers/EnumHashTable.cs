namespace Genbox.FastEnum.Helpers;

internal sealed class EnumHashTable
{
    private EnumHashTable(int[] buckets, int[] next, int shift)
    {
        Buckets = buckets;
        Next = next;
        Shift = shift;
    }

    internal int[] Buckets { get; }
    internal int[] Next { get; }
    internal int Shift { get; }

    internal static EnumHashTable Create(ulong[] values)
    {
        int size = 1;
        while (size < values.Length)
            size <<= 1;

        int mask = size - 1;
        int[] counts = new int[size];
        int bestShift = 0;
        long bestCost = long.MaxValue;

        // Choose the cheapest bit slice for these constants, including high-bit-only values.
        for (int shift = 0; shift < 64; shift++)
        {
            Array.Clear(counts, 0, counts.Length);
            long cost = 0;
            foreach (ulong value in values)
                cost += counts[(int)((value >> shift) & (uint)mask)]++;

            if (cost >= bestCost)
                continue;

            bestCost = cost;
            bestShift = shift;
            if (cost == 0)
                break;
        }

        int[] buckets = Enumerable.Repeat(-1, size).ToArray();
        int[] next = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            int bucket = (int)((values[i] >> bestShift) & (uint)mask);
            next[i] = buckets[bucket];
            buckets[bucket] = i;
        }

        return new EnumHashTable(buckets, next, bestShift);
    }
}