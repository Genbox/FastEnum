using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Length")]
public class EnumLengthBenchmark
{
    [Benchmark(Baseline = true)]
    public int EnumLength() => Enum.GetNames(typeof(TestEnum)).Length;

    [Benchmark]
    public int CodeGenLength() => Enums.TestEnum.MemberCount;

    [Benchmark]
    public int EnumsNetLength() => EnumsNET.Enums.GetMemberCount(typeof(TestEnum));
}