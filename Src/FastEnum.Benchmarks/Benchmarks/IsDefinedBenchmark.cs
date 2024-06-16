using Genbox.FastEnum.Benchmarks.Code;

namespace Genbox.FastEnum.Benchmarks.Benchmarks;

[BenchmarkCategory("IsDefined")]
public class IsDefinedBenchmark
{
    private static readonly TestEnum _enum = TestEnum.Second;
    private static readonly TestFlagsEnum _flagsEnum = TestFlagsEnum.One;

    [Benchmark(Baseline = true)]
    public bool EnumIsDefined() => Enum.IsDefined(typeof(TestFlagsEnum), _enum);

    [Benchmark]
    public bool CodeGenIsDefined() => Enums.TestEnum.IsDefined(_enum);

    [Benchmark]
    public bool EnumsNetIsDefined() => EnumsNET.Enums.IsDefined(_enum);

    [Benchmark]
    public bool EnumIsDefinedFlags() => Enum.IsDefined(typeof(TestFlagsEnum), _flagsEnum);

    [Benchmark]
    public bool CodeGenIsDefinedFlags() => Enums.TestFlagsEnum.IsDefined(_flagsEnum);

    [Benchmark]
    public bool EnumsNetIsDefinedFlags() => EnumsNET.Enums.IsDefined(_flagsEnum);
}