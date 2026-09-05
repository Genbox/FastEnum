using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;
using Enums = Genbox.FastEnum.Benchmarks.Code.Enums;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParseDisplayNameLarge")]
public class LargeDisplayNameTryParseBenchmark
{
    [Params("Last value", "Missing")]
    public string Input { get; set; } = null!;

    [Benchmark(Baseline = true)]
    public bool ReflectionTryParse() => EnumHelper<LargeEnum>.TryParseByDisplayName(Input, false, out _);

    [Benchmark]
    public bool FastEnumTryParse() => Enums.LargeEnum.TryParse(Input, out _, LargeEnumFormat.DisplayName);

    [Benchmark]
    public bool EnumsNetTryParse() => EnumsNET.Enums.TryParse<LargeEnum>(Input, false, out _, EnumFormat.DisplayName);
}