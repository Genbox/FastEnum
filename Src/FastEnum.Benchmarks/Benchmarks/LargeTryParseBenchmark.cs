using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParseLarge")]
public class LargeTryParseBenchmark
{
    [Params("Value0", "Value1023", "Missing", "value1023")]
    public string Input { get; set; } = null!;

    [Params(false, true)]
    public bool IgnoreCase { get; set; }

    private StringComparison Comparison => IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    [Benchmark(Baseline = true)]
    public bool EnumTryParse() => Enum.TryParse<LargeEnum>(Input, IgnoreCase, out _);

    [Benchmark]
    public bool FastEnumTryParse() => Enums.LargeEnum.TryParse(Input, out _, comparison: Comparison);

    [Benchmark]
    public bool EnumsNetTryParse() => EnumsNET.Enums.TryParse<LargeEnum>(Input, IgnoreCase, out _);

    [Benchmark]
    public bool EnumTryParseSpan() => Enum.TryParse<LargeEnum>(Input.AsSpan(), IgnoreCase, out _);

    [Benchmark]
    public bool FastEnumTryParseSpan() => Enums.LargeEnum.TryParse(Input.AsSpan(), out _, comparison: Comparison);
}