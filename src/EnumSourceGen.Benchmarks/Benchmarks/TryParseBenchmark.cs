using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.EnumSourceGen.Benchmarks.Code;
using Enums = Genbox.EnumSourceGen.Benchmarks.Code.Enums;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParse")]
public class TryParseBenchmark
{
    [Benchmark(Baseline = true)]
    public TestEnum EnumTryParse() => Enum.TryParse("Second", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum CodeGenTryParse() => Enums.TestEnum.TryParse("Second", out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum EnumsNetTryParse() => EnumsNET.Enums.TryParse("Second", true, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum ReflectionTryParseDisplayName() => EnumHelper<TestEnum>.TryParseByDisplayName("2nd", false, out TestEnum result) ? result : default;

    [Benchmark]
    public TestEnum CodeGenTryParseDisplayName() => Enums.TestEnum.TryParse("2nd", out TestEnum result, TestEnumFormat.DisplayName) ? result : default;

    [Benchmark]
    public TestEnum EnumsNetTryParseDisplayName() => EnumsNET.Enums.TryParse("2nd", true, out TestEnum result, EnumFormat.DisplayName) ? result : default;
}