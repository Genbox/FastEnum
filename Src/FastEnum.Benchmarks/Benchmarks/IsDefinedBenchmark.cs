using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("IsDefined")]
public class IsDefinedBenchmark
{
    [Params(TestEnum.First, TestEnum.Third, (TestEnum)(-1))]
    public TestEnum Value { get; set; }

    [Benchmark(Baseline = true)]
    public bool EnumIsDefined() => Enum.IsDefined(Value);

    [Benchmark]
    public bool EnumIsDefinedNonGeneric() => Enum.IsDefined(typeof(TestEnum), Value);

    [Benchmark]
    public bool FastEnumIsDefined() => Enums.TestEnum.IsDefined(Value);

    [Benchmark]
    public bool EnumsNetIsDefined() => EnumsNET.Enums.IsDefined(Value);
}