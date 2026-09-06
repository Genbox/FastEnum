using Genbox.FastEnum.Benchmarks.Code;
using Enums = EnumsNET.Enums;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("Underlying values")]
public class GetUnderlyingValueBenchmark
{
    [Params(TestEnum.First, TestEnum.Third)]
    public TestEnum Value { get; set; }

    [Params((LargeEnum)0, LargeEnum.Value1023)]
    public LargeEnum LargeValue { get; set; }

    [Benchmark(Baseline = true)]
    public int EnumGetValues() => (int)Value;

    [Benchmark]
    public int FastEnumGetValues() => Value.GetUnderlyingValue();

    [Benchmark]
    public int EnumsNetGetValues() => (int)Enums.GetUnderlyingValue(Value);

    [Benchmark]
    public int EnumGetValuesLargeEnum() => (int)LargeValue;

    [Benchmark]
    public int FastEnumGetValuesLargeEnum() => LargeValue.GetUnderlyingValue();

    [Benchmark]
    public int EnumsNetGetValuesLargeEnum() => (int)Enums.GetUnderlyingValue(LargeValue);
}