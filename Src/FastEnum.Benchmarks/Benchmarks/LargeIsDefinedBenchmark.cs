using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("IsDefinedLarge")]
public class LargeIsDefinedBenchmark
{
    [Params((LargeEnum)0, LargeEnum.Value1023, (LargeEnum)(-1))]
    public LargeEnum Value { get; set; }

    [Benchmark(Baseline = true)]
    public bool EnumIsDefined() => Enum.IsDefined(Value);

    [Benchmark]
    public bool FastEnumIsDefined() => Enums.LargeEnum.IsDefined(Value);

    [Benchmark]
    public bool EnumsNetIsDefined() => EnumsNET.Enums.IsDefined(Value);
}