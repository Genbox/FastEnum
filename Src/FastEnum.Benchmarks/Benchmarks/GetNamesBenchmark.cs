using BenchmarkDotNet.Attributes;
using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Names")]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    public string[] EnumGetNames() => Enum.GetNames<TestEnum>();

    [Benchmark]
    public string[] CodeGenGetNames() => Enums.TestEnum.GetMemberNames();

    [Benchmark]
    public IReadOnlyList<string> EnumsNetGetNames() => EnumsNET.Enums.GetNames<TestEnum>();
}