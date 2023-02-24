using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("TryParse")]
public class TryParseBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumTryParse() => Enum.TryParse("Second", false, out TestEnum result) ? result : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum CodeGenTryParse() => Enums.TestEnum.TryParse("Second", out TestEnum result, StringComparison.OrdinalIgnoreCase) ? result : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumsNetTryParse() => EnumsNET.Enums.TryParse("Second", true, out TestEnum result) ? result : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum ReflectionTryParseDisplayName() => EnumHelper<TestEnum>.TryParseByDisplayName("2nd", false, out TestEnum result) ? result : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum CodeGenTryParseDisplayName() => Enums.TestEnum.TryParse("2nd", out TestEnum result, StringComparison.OrdinalIgnoreCase) ? result : default;

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum EnumsNetTryParseDisplayName() => EnumsNET.Enums.TryParse("2nd", true, out TestEnum result, EnumFormat.DisplayName) ? result : default;
}