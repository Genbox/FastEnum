using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Flags")]
public class FlagBenchmarks
{
    private static readonly TestFlagsEnum _flags = TestFlagsEnum.One | TestFlagsEnum.Two;

    [Benchmark(Baseline = true)]
    public bool EnumHasFlag() => _flags.HasFlag(TestFlagsEnum.One);

    [Benchmark]
    public bool CodeGenHasFlag() => _flags.IsFlagSet(TestFlagsEnum.One);

    [Benchmark]
    public bool EnumsNetHasFlag() => _flags.HasAnyFlags(TestFlagsEnum.One);
}