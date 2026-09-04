using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Length")]
public class EnumLengthBenchmark
{
    [Benchmark(Baseline = true)]
    public int EnumLength() => Enum.GetNames(typeof(TestEnum)).Length;

    [Benchmark]
    public int FastEnumLength() => Enums.TestEnum.MemberCount;

    [Benchmark]
    public int EnumsNetLength() => EnumsNET.Enums.GetMemberCount(typeof(TestEnum));

    [Benchmark]
    public int EnumLengthLargeEnum() => Enum.GetNames(typeof(LargeEnum)).Length;

    [Benchmark]
    public int FastEnumLengthLargeEnum() => Enums.LargeEnum.MemberCount;

    [Benchmark]
    public int EnumsNetLengthLargeEnum() => EnumsNET.Enums.GetMemberCount(typeof(LargeEnum));
}