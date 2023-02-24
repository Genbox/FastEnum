using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Values")]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] EnumGetValues() => Enum.GetValues<TestEnum>();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] CodeGenGetValues() => Enums.TestEnum.GetMemberValues();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public IReadOnlyList<TestEnum> EnumsNetGetValues() => EnumsNET.Enums.GetValues<TestEnum>();
}