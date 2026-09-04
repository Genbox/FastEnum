using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Names")]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    public string[] EnumGetNames() => Enum.GetNames<TestEnum>();

    [Benchmark]
    public string[] FastEnumGetNames() => Enums.TestEnum.GetMemberNames();

    [Benchmark]
    public IReadOnlyList<string> EnumsNetGetNames() => EnumsNET.Enums.GetNames<TestEnum>();

    [Benchmark]
    public string[] EnumGetNamesLargeEnum() => Enum.GetNames<LargeEnum>();

    [Benchmark]
    public string[] FastEnumGetNamesLargeEnum() => Enums.LargeEnum.GetMemberNames();

    [Benchmark]
    public IReadOnlyList<string> EnumsNetGetNamesLargeEnum() => EnumsNET.Enums.GetNames<LargeEnum>();
}