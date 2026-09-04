using System.Diagnostics.CodeAnalysis;
using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[SuppressMessage("Performance", "CA1802:Use literals where appropriate", Justification = "Compiler will tamper with results if const")]
[BenchmarkCategory("ToString")]
public class ToStringBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;
    private static readonly LargeEnum _largeEnum = LargeEnum.Value1023;

    [Benchmark(Baseline = true)]
    public string EnumToString() => _enum.ToString();

    [Benchmark]
    public string FastEnumToString() => _enum.GetString();

    [Benchmark]
    public string EnumsNetToString() => _enum.AsString();

    [Benchmark]
    public string FastEnumGetDisplayName() => _enum.GetDisplayName();

    [Benchmark]
    public string? EnumsNetGetDisplayName() => _enum.AsString(EnumFormat.DisplayName);

    [Benchmark]
    public string ReflectionGetDisplayName() => EnumHelper<TestEnum>.GetDisplayName(_enum);

    [Benchmark]
    public string EnumToStringLargeEnum() => _largeEnum.ToString();

    [Benchmark]
    public string FastEnumToStringLargeEnum() => _largeEnum.GetString();

    [Benchmark]
    public string EnumsNetToStringLargeEnum() => _largeEnum.AsString();

    [Benchmark]
    public string FastEnumGetDisplayNameLargeEnum() => _largeEnum.GetDisplayName();

    [Benchmark]
    public string? EnumsNetGetDisplayNameLargeEnum() => _largeEnum.AsString(EnumFormat.DisplayName);

    [Benchmark]
    public string ReflectionGetDisplayNameLargeEnum() => EnumHelper<LargeEnum>.GetDisplayName(_largeEnum);
}