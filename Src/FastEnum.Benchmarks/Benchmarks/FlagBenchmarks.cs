using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Flags")]
public class FlagBenchmarks
{
    [Params(TestFlagsEnum.None, TestFlagsEnum.One, TestFlagsEnum.One | TestFlagsEnum.Two)]
    public TestFlagsEnum Value { get; set; }

    [Benchmark(Baseline = true)]
    public bool EnumHasFlag() => Value.HasFlag(TestFlagsEnum.One);

    [Benchmark]
    public bool FastEnumHasFlag() => Value.IsFlagSet(TestFlagsEnum.One);

    [Benchmark]
    public bool EnumsNetHasFlag() => Value.HasAnyFlags(TestFlagsEnum.One);
}