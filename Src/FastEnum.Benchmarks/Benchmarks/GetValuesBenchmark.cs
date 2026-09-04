using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Values")]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    public TestEnum[] EnumGetValues() => Enum.GetValues<TestEnum>();

    [Benchmark]
    public TestEnum[] FastEnumGetValues() => Enums.TestEnum.GetMemberValues();

    [Benchmark]
    public IReadOnlyList<TestEnum> EnumsNetGetValues() => EnumsNET.Enums.GetValues<TestEnum>();

    [Benchmark]
    public LargeEnum[] EnumGetValuesLargeEnum() => Enum.GetValues<LargeEnum>();

    [Benchmark]
    public LargeEnum[] FastEnumGetValuesLargeEnum() => Enums.LargeEnum.GetMemberValues();

    [Benchmark]
    public IReadOnlyList<LargeEnum> EnumsNetGetValuesLargeEnum() => EnumsNET.Enums.GetValues<LargeEnum>();
}