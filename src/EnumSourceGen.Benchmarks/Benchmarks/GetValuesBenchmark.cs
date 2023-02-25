﻿using BenchmarkDotNet.Attributes;
using Genbox.EnumSourceGen.Benchmarks.Code;

namespace Genbox.EnumSourceGen.Benchmarks.Benchmarks;

[BenchmarkCategory("Values")]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    public TestEnum[] EnumGetValues() => Enum.GetValues<TestEnum>();

    [Benchmark]
    public TestEnum[] CodeGenGetValues() => Enums.TestEnum.GetMemberValues();

    [Benchmark]
    public IReadOnlyList<TestEnum> EnumsNetGetValues() => EnumsNET.Enums.GetValues<TestEnum>();
}