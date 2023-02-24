using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Underlying values")]
public class GetUnderlyingValueBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumGetValues() => (int)_enum;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int CodeGenGetValues() => _enum.GetUnderlyingValue();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumsNetGetValues() => (int)EnumsNET.Enums.GetUnderlyingValue(_enum);
}