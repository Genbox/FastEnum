using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Names")]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetNames() => Enum.GetNames<TestEnum>();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] CodeGenGetNames() => Enums.TestEnum.GetMemberNames();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public IReadOnlyList<string> EnumsNetGetNames() => EnumsNET.Enums.GetNames<TestEnum>();
}