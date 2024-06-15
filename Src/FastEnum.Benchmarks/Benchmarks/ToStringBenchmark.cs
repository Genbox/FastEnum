using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("ToString")]
public class ToStringBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    public string EnumToString() => _enum.ToString();

    [Benchmark]
    public string CodeGenToString() => _enum.GetString();

    [Benchmark]
    public string EnumsNetToString() => _enum.AsString();

    [Benchmark]
    public string? CodeGenGetDisplayName() => _enum.GetDisplayName();

    [Benchmark]
    public string? EnumsNetGetDisplayName() => _enum.AsString(EnumFormat.DisplayName);

    [Benchmark]
    public string ReflectionGetDisplayName() => EnumHelper<TestEnum>.GetDisplayName(_enum);
}