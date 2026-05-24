using System.Diagnostics.CodeAnalysis;
using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[SuppressMessage("Performance", "CA1802:Use literals where appropriate", Justification = "Compiler will tamper with results if const")]
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