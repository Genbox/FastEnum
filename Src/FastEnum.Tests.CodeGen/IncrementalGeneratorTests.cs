using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Genbox.FastEnum.Tests.CodeGen;

public class IncrementalGeneratorTests
{
    private const string Original = "[Genbox.FastEnum.FastEnum] public enum Color { Red, Green }";

    [Fact]
    public void UnrelatedEditReusesCollectedEnumsAndGeneratedOutput()
    {
        SyntaxTree unrelated = Parse("public class Unrelated { }");
        CSharpCompilation compilation = TestHelper.CreateCompilation([Parse(Original), unrelated]);
        GeneratorDriver driver = CreateDriver().RunGenerators(compilation, TestContext.Current.CancellationToken);
        GeneratorDriverRunResult before = driver.GetRunResult();

        compilation = compilation.ReplaceSyntaxTree(unrelated, Parse("public class Unrelated { public int Value; }"));
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        GeneratorRunResult result = Assert.Single(driver.GetRunResult().Results);

        Assert.Empty(result.Diagnostics);
        Assert.All(result.TrackedSteps["EnumSpecs"].SelectMany(x => x.Outputs),
            output => Assert.True(output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged));
        Assert.All(result.TrackedSteps["CollectedEnums"].SelectMany(x => x.Outputs),
            output => Assert.Equal(IncrementalStepRunReason.Cached, output.Reason));
        Assert.All(result.TrackedOutputSteps.SelectMany(x => x.Value).SelectMany(x => x.Outputs),
            output => Assert.Equal(IncrementalStepRunReason.Cached, output.Reason));
        Assert.Equal(Sources(before.Results[0]), Sources(result));
    }

    [Theory]
    [InlineData("[Genbox.FastEnum.FastEnum] public enum Color { Red, Green, Blue }")]
    [InlineData("[Genbox.FastEnum.FastEnum] public enum Color { Red = 42, Green }")]
    [InlineData("[Genbox.FastEnum.FastEnum] public enum Color { [System.ComponentModel.DataAnnotations.Display(Name = \"Rouge\")] Red, Green }")]
    [InlineData("[Genbox.FastEnum.FastEnum] public enum Color { [Genbox.FastEnum.EnumOmitValue] Red, Green }")]
    [InlineData("[Genbox.FastEnum.FastEnum(DisableCache = true)] public enum Color { Red, Green }")]
    [InlineData("[Genbox.FastEnum.FastEnum, Genbox.FastEnum.EnumTransform(Preset = Genbox.FastEnum.EnumTransform.UpperCase)] public enum Color { Red, Green }")]
    public void RelevantEditInvalidatesSpecAndUpdatesOutput(string updated)
    {
        SyntaxTree original = Parse(Original);
        SyntaxTree other = Parse("[Genbox.FastEnum.FastEnum] public enum Size { Small, Large }");
        CSharpCompilation compilation = TestHelper.CreateCompilation([original, other]);
        GeneratorDriver driver = CreateDriver().RunGenerators(compilation, TestContext.Current.CancellationToken);
        GeneratorRunResult before = Assert.Single(driver.GetRunResult().Results);

        compilation = compilation.ReplaceSyntaxTree(original, Parse(updated));
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out var diagnostics, TestContext.Current.CancellationToken);
        GeneratorRunResult after = Assert.Single(driver.GetRunResult().Results);

        Assert.Empty(diagnostics);
        Assert.Empty(outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken).Where(x => x.Severity == DiagnosticSeverity.Error));
        Assert.Contains(after.TrackedSteps["EnumSpecs"].SelectMany(x => x.Outputs), x => x.Reason == IncrementalStepRunReason.Modified);
        Assert.Contains(after.TrackedSteps["EnumSpecs"].SelectMany(x => x.Outputs), x => x.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
        Assert.NotEqual(Sources(before), Sources(after));
        Assert.Equal(Sources(before, "Size_"), Sources(after, "Size_"));
    }

    [Fact]
    public void FileLocalEditInvalidatesPreviouslyValidSpec()
    {
        SyntaxTree original = Parse("[Genbox.FastEnum.FastEnum] internal enum Color { Red }");
        CSharpCompilation compilation = TestHelper.CreateCompilation([original]);
        GeneratorDriver driver = CreateDriver().RunGenerators(compilation, TestContext.Current.CancellationToken);
        compilation = compilation.ReplaceSyntaxTree(original, Parse("[Genbox.FastEnum.FastEnum] file enum Color { Red }"));
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        GeneratorRunResult result = Assert.Single(driver.GetRunResult().Results);
        Assert.Equal("FE001", Assert.Single(result.Diagnostics).Id);
        Assert.Empty(result.GeneratedSources);
        compilation = compilation.ReplaceSyntaxTree(compilation.SyntaxTrees.Single(), original);
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        result = Assert.Single(driver.GetRunResult().Results);
        Assert.Empty(result.Diagnostics);
        Assert.Equal(3, result.GeneratedSources.Length);
    }

    private static SyntaxTree Parse(string source) => CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken);

    private static CSharpGeneratorDriver CreateDriver() => CSharpGeneratorDriver.Create(
        [new EnumGenerator().AsSourceGenerator()],
        driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));

    private static string[] Sources(GeneratorRunResult result, string prefix = "") => result.GeneratedSources
        .Where(x => x.HintName.StartsWith(prefix, StringComparison.Ordinal))
        .OrderBy(x => x.HintName, StringComparer.Ordinal)
        .Select(x => x.SourceText.ToString()).ToArray();
}