using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("ToString")]
public class ToStringBenchmark
{
    // These members have display metadata, which the display-name benchmarks require.
    [Params(TestEnum.Second)]
    public TestEnum Value { get; set; }

    [Params(LargeEnum.Value1023)]
    public LargeEnum LargeValue { get; set; }

    [Benchmark(Baseline = true)]
    public string EnumToString() => Value.ToString();

    [Benchmark]
    public string FastEnumToString() => Value.GetString();

    [Benchmark]
    public string EnumsNetToString() => Value.AsString();

    [Benchmark]
    public string FastEnumGetDisplayName() => Value.GetDisplayName();

    [Benchmark]
    public string? EnumsNetGetDisplayName() => Value.AsString(EnumFormat.DisplayName);

    [Benchmark]
    public string ReflectionGetDisplayName() => EnumHelper<TestEnum>.GetDisplayName(Value);

    [Benchmark]
    public string EnumToStringLargeEnum() => LargeValue.ToString();

    [Benchmark]
    public string FastEnumToStringLargeEnum() => LargeValue.GetString();

    [Benchmark]
    public string EnumsNetToStringLargeEnum() => LargeValue.AsString();

    [Benchmark]
    public string FastEnumGetDisplayNameLargeEnum() => LargeValue.GetDisplayName();

    [Benchmark]
    public string? EnumsNetGetDisplayNameLargeEnum() => LargeValue.AsString(EnumFormat.DisplayName);

    [Benchmark]
    public string ReflectionGetDisplayNameLargeEnum() => EnumHelper<LargeEnum>.GetDisplayName(LargeValue);
}