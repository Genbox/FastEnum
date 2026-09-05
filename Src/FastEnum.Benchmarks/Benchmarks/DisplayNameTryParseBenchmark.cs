using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;
using Enums = Genbox.FastEnum.Benchmarks.Code.Enums;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParseDisplayName")]
public class DisplayNameTryParseBenchmark
{
    [Params("2nd", "Missing")]
    public string Input { get; set; } = null!;

    [Benchmark(Baseline = true)]
    public bool ReflectionTryParse() => EnumHelper<TestEnum>.TryParseByDisplayName(Input, false, out _);

    [Benchmark]
    public bool FastEnumTryParse() => Enums.TestEnum.TryParse(Input, out _, TestEnumFormat.DisplayName);

    [Benchmark]
    public bool EnumsNetTryParse() => EnumsNET.Enums.TryParse<TestEnum>(Input, false, out _, EnumFormat.DisplayName);
}