using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("IsDefined")]
public class IsDefinedLookupBenchmark
{
    [Params(TestEnum.First, TestEnum.Third, (TestEnum)3)]
    public TestEnum Value { get; set; }

    [Benchmark(Baseline = true)]
    public bool PreviousArrayLookup()
    {
        int[] values = Enums.TestEnum.GetUnderlyingValues();

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == (int)Value)
                return true;
        }

        return false;
    }

    [Benchmark]
    public bool GeneratedLookup() => Enums.TestEnum.IsDefined(Value);
}