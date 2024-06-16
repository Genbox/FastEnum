using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Underlying values")]
public class GetUnderlyingValueBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    public int EnumGetValues() => (int)_enum;

    [Benchmark]
    public int CodeGenGetValues() => _enum.GetUnderlyingValue();

    [Benchmark]
    public int EnumsNetGetValues() => (int)EnumsNET.Enums.GetUnderlyingValue(_enum);
}