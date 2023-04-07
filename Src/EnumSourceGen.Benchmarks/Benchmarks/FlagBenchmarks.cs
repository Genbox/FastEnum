using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Flags")]
public class FlagBenchmarks
{
    private static readonly TestEnum _flags = TestEnum.First | TestEnum.Second;

    [Benchmark(Baseline = true)]
    public bool EnumHasFlag() => _flags.HasFlag(TestEnum.First);

    [Benchmark]
    public bool CodeGenHasFlag() => _flags.IsFlagSet(TestEnum.First);

    [Benchmark]
    public bool EnumsNetHasFlag() => _flags.HasAnyFlags(TestEnum.First);
}