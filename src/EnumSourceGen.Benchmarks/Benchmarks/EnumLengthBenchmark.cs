using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Length")]
public class EnumLengthBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumLength() => Enum.GetNames(typeof(TestEnum)).Length;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int CodeGenLength() => Enums.TestEnum.GetMemberNames().Length;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int CodeGenConstLength() => Enums.TestEnum.MemberCount;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumsNetConstLength() => EnumsNET.Enums.GetMemberCount(typeof(TestEnum));
}