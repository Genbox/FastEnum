using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Flags")]
public class FlagBenchmarks
{
    private static TestEnum _flags = TestEnum.First | TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumHasFlag() => _flags.HasFlag(TestEnum.First);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool CodeGenHasFlag() => _flags.IsFlagSet(TestEnum.First);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumsNetHasAnyFlags() => _flags.HasAnyFlags(TestEnum.First);
}