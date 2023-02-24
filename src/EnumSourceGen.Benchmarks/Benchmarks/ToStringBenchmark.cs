using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("ToString")]
public class ToStringBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumToString() => _enum.ToString();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string CodeGenToString() => _enum.GetString();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumsNetToString() => EnumsNET.Enums.AsString(_enum);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string? CodeGenGetDisplayName() => _enum.GetDisplayName();

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string? EnumsNetGetDisplayName() => EnumsNET.Enums.AsString(_enum, EnumFormat.DisplayName);

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string ReflectionGetDisplayName() => EnumHelper<TestEnum>.GetDisplayName(_enum);
}