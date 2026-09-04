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
    public TestEnum FastEnumTryParse() => Enums.TestEnum.TryParse("Second", out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum EnumsNetTryParse() => EnumsNET.Enums.TryParse("Second", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum ReflectionTryParseDisplayName() => EnumHelper<TestEnum>.TryParseByDisplayName("2nd", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum FastEnumTryParseDisplayName() => Enums.TestEnum.TryParse("2nd", out TestEnum result, TestEnumFormat.DisplayName) ? result : default;

    [Benchmark]
    public TestEnum EnumsNetTryParseDisplayName() => EnumsNET.Enums.TryParse("2nd", false, out TestEnum result, EnumFormat.DisplayName) ? result : default;

    [Benchmark]
    public LargeEnum EnumTryParseLargeEnum() => Enum.TryParse("Value1023", false, out LargeEnum result) ? result : default;

    [Benchmark]
    public LargeEnum FastEnumTryParseLargeEnum() => Enums.LargeEnum.TryParse("Value1023", out LargeEnum result) ? result : default;

    [Benchmark]
    public LargeEnum EnumsNetTryParseLargeEnum() => EnumsNET.Enums.TryParse("Value1023", false, out LargeEnum result) ? result : default;

    [Benchmark]
    public LargeEnum ReflectionTryParseDisplayNameLargeEnum() => EnumHelper<LargeEnum>.TryParseByDisplayName("Last value", false, out LargeEnum result) ? result : default;

    [Benchmark]
    public LargeEnum FastEnumTryParseDisplayNameLargeEnum() => Enums.LargeEnum.TryParse("Last value", out LargeEnum result, LargeEnumFormat.DisplayName) ? result : default;

    [Benchmark]
    public LargeEnum EnumsNetTryParseDisplayNameLargeEnum() => EnumsNET.Enums.TryParse("Last value", false, out LargeEnum result, EnumFormat.DisplayName) ? result : default;
}