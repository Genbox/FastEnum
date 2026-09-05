using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("IsDefinedFlags")]
public class FlagsIsDefinedBenchmark
{
    // Only named values and unknown bits: composite semantics differ between these APIs.
    [Params(TestFlagsEnum.None, TestFlagsEnum.Two, (TestFlagsEnum)4)]
    public TestFlagsEnum Value { get; set; }

    [Benchmark(Baseline = true)]
    public bool EnumIsDefined() => Enum.IsDefined(Value);

    [Benchmark]
    public bool FastEnumIsDefined() => Enums.TestFlagsEnum.IsDefined(Value);

    [Benchmark]
    public bool EnumsNetIsDefined() => EnumsNET.Enums.IsDefined(Value);
}