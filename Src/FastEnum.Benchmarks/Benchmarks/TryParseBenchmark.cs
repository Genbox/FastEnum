using EnumsNET;
using Genbox.FastEnum.Benchmarks.Code;
using Enums = Genbox.FastEnum.Benchmarks.Code.Enums;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParse")]
public class TryParseBenchmark
{
    [Benchmark(Baseline = true)]
    public TestEnum EnumTryParse() => Enum.TryParse("Second", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum CodeGenTryParse() => Enums.TestEnum.TryParse("Second", out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum EnumsNetTryParse() => EnumsNET.Enums.TryParse("Second", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum ReflectionTryParseDisplayName() => EnumHelper<TestEnum>.TryParseByDisplayName("2nd", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum CodeGenTryParseDisplayName() => Enums.TestEnum.TryParse("2nd", out TestEnum result, TestEnumFormat.DisplayName) ? result : default;

    [Benchmark]
    public TestEnum EnumsNetTryParseDisplayName() => EnumsNET.Enums.TryParse("2nd", false, out TestEnum result, EnumFormat.DisplayName) ? result : default;
}