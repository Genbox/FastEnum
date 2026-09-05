using Genbox.FastEnum.Benchmarks.Code;
using Enums = Genbox.FastEnum.Benchmarks.Code.Enums;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParse")]
public class TryParseBenchmark
{
    [Params("First", "Third", "Missing", "third")]
    public string Input { get; set; } = null!;

    [Params(false, true)]
    public bool IgnoreCase { get; set; }

    private StringComparison Comparison => IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    [Benchmark(Baseline = true)]
    public bool EnumTryParse() => Enum.TryParse<TestEnum>(Input, IgnoreCase, out _);

    [Benchmark]
    public bool FastEnumTryParse() => Enums.TestEnum.TryParse(Input, out _, comparison: Comparison);

    [Benchmark]
    public bool EnumsNetTryParse() => EnumsNET.Enums.TryParse<TestEnum>(Input, IgnoreCase, out _);

    [Benchmark]
    public bool EnumTryParseSpan() => Enum.TryParse<TestEnum>(Input.AsSpan(), IgnoreCase, out _);

    [Benchmark]
    public bool FastEnumTryParseSpan() => Enums.TestEnum.TryParse(Input.AsSpan(), out _, comparison: Comparison);
}