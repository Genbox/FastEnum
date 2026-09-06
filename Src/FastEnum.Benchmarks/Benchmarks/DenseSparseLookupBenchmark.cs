using System.Globalization;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Configs;
using Genbox.FastEnum.Benchmarks.Code;
using Genbox.FastEnum.Helpers;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class DenseSparseLookupBenchmark
{
    private const int OperationCount = 1024;
    private const int MemberCount = 32;
    private int[] inputs = null!;

    [Params(false, true)]
    public bool Sparse { get; set; }

    [Params(false, true)]
    public bool Hits { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        int stride = Sparse ? 3 : 1;
        inputs = new int[OperationCount];

        for (int i = 0; i < inputs.Length; i++)
        {
            int memberIndex = i % MemberCount;
            int value = Hits ? memberIndex : -1 - memberIndex;
            inputs[i] = value * stride;
        }
    }

    // Keep dispatch outside the measured loops and avoid delegates in the lookup hot path.
    // The previous generator used this chained hash lookup for dense and sparse values alike.
    [Benchmark(Baseline = true, OperationsPerInvoke = OperationCount), BenchmarkCategory("IsDefined")]
    public int PreviousIsDefined()
    {
        int count = 0;

        if (Sparse)
        {
            foreach (int input in inputs)
            {
                if (PreviousLookup<SparseLookupEnum>.Contains(input))
                    count++;
            }
        }
        else
        {
            foreach (int input in inputs)
            {
                if (PreviousLookup<DenseLookupEnum>.Contains(input))
                    count++;
            }
        }

        return count;
    }

    [Benchmark(OperationsPerInvoke = OperationCount), BenchmarkCategory("IsDefined")]
    public int GeneratedIsDefined()
    {
        int count = 0;

        if (Sparse)
        {
            foreach (int input in inputs)
            {
                if (Enums.SparseLookupEnum.IsDefined((SparseLookupEnum)input))
                    count++;
            }
        }
        else
        {
            foreach (int input in inputs)
            {
                if (Enums.DenseLookupEnum.IsDefined((DenseLookupEnum)input))
                    count++;
            }
        }

        return count;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = OperationCount), BenchmarkCategory("Text")]
    public int PreviousGetString()
    {
        int length = 0;

        if (Sparse)
        {
            foreach (int input in inputs)
                length += (PreviousLookup<SparseLookupEnum>.Find(input) ?? ((SparseLookupEnum)input).ToString()).Length;
        }
        else
        {
            foreach (int input in inputs)
                length += (PreviousLookup<DenseLookupEnum>.Find(input) ?? ((DenseLookupEnum)input).ToString()).Length;
        }

        return length;
    }

    [Benchmark(OperationsPerInvoke = OperationCount), BenchmarkCategory("Text")]
    public int GeneratedGetString()
    {
        int length = 0;

        if (Sparse)
        {
            foreach (int input in inputs)
                length += ((SparseLookupEnum)input).GetString().Length;
        }
        else
        {
            foreach (int input in inputs)
                length += ((DenseLookupEnum)input).GetString().Length;
        }

        return length;
    }

    private static class PreviousLookup<T> where T : struct, Enum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Contains(int input)
        {
            for (int index = buckets[input & (buckets.Length - 1)]; index >= 0; index = next[index])
            {
                if (values[index] == input)
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string? Find(int input)
        {
            for (int index = buckets[input & (buckets.Length - 1)]; index >= 0; index = next[index])
            {
                if (values[index] == input)
                    return names[index];
            }

            return null;
        }
#pragma warning disable S2743 // Separate per-enum tables reproduce the generated static lookup holders.
        private static readonly int[] values = Enum.GetValues<T>().Select(value => Convert.ToInt32(value, CultureInfo.InvariantCulture)).ToArray();
        private static readonly string[] names = Enum.GetNames<T>();
        private static readonly EnumHashTable table = EnumHashTable.Create(values.Select(i => (ulong)i).ToArray());
        private static readonly int[] buckets = table.Buckets;
        private static readonly int[] next = table.Next;
#pragma warning restore S2743
    }
}