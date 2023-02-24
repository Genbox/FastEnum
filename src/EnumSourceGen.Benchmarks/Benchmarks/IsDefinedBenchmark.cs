using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("IsDefined")]
public class IsDefinedBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined() => Enum.IsDefined(typeof(TestEnum), _enum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool CodeGenIsDefined() => Enums.TestEnum.IsDefined(_enum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumsNetIsDefined() => EnumsNET.Enums.IsDefined(_enum);
}