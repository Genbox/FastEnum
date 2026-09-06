using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Genbox.FastEnum.Tests.CodeGen;

public class DenseLookupTests
{
    private const int DenseMemberCount = 32;

    [Theory]
    [InlineData("sbyte", "-128", "127")]
    [InlineData("sbyte", "96", "-128")]
    [InlineData("byte", "224", "0")]
    [InlineData("short", "-32768", "32767")]
    [InlineData("ushort", "65504", "0")]
    [InlineData("int", "-2147483648", "2147483647")]
    [InlineData("int", "2147483616", "-2147483648")]
    [InlineData("int", "-16", "2147483647")]
    [InlineData("uint", "4294967264", "0")]
    [InlineData("long", "-9223372036854775808", "9223372036854775807")]
    [InlineData("long", "9223372036854775776", "-9223372036854775808")]
    [InlineData("long", "-16", "9223372036854775807")]
    [InlineData("ulong", "18446744073709551584", "0")]
    public void DenseLookupMatchesValuesAndMetadataAtNumericBoundaries(string underlyingType, string firstValueText, string missingValueText)
    {
        decimal firstValue = decimal.Parse(firstValueText, CultureInfo.InvariantCulture);

        // Reverse declaration order and add an alias so array indexing cannot rely on source order.
        string members = string.Join(",\n", Enumerable.Range(0, DenseMemberCount).Reverse().Select(i => $$"""
                                                                                                          [System.ComponentModel.DataAnnotations.Display(Name = "Label{{i}}")]
                                                                                                          Value{{i}} = {{(firstValue + i).ToString(CultureInfo.InvariantCulture)}}
                                                                                                          """));
        string assertions = string.Join('\n', Enumerable.Range(0, DenseMemberCount).Select(i => $$"""
                                                                                                  if (!Enums.Sample.IsDefined(Sample.Value{{i}}) || Sample.Value{{i}}.GetString() != "Value{{i}}" ||
                                                                                                      Sample.Value{{i}}.GetDisplayName() != "Label{{i}}" ||
                                                                                                      !Sample.Value{{i}}.TryGetUnderlyingValue(out var raw{{i}}) || raw{{i}} != ({{underlyingType}})Sample.Value{{i}})
                                                                                                      return false;
                                                                                                  """));

        string source = $$"""
                          [Genbox.FastEnum.FastEnum]
                          [Genbox.FastEnum.EnumTransform(Preset = Genbox.FastEnum.EnumTransform.None)]
                          public enum Sample : {{underlyingType}}
                          {
                              {{members}},
                              Alias = Value0
                          }

                          public static class Probe
                          {
                              public static bool Run()
                              {
                                  {{assertions}}
                                  Sample missing = (Sample)({{missingValueText}});
                                  return !Enums.Sample.IsDefined(missing) && !missing.TryGetUnderlyingValue(out _) &&
                                      !missing.TryGetDisplayName(out _) && missing.GetString() == missing.ToString();
                              }
                          }
                          """;

        Assert.True(CompileAndRun(source));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OmittedValuesAndAliasesPreserveLookupSemantics(bool disableCache)
    {
        // An omission in the middle leaves a hole, while the alias must not add another slot.
        string members = string.Join(",\n", Enumerable.Range(0, 34).Select(i =>
            (i == 16 ? "[Genbox.FastEnum.EnumOmitValue] " : "") + $"Value{i} = {i}"));
        string source = $$"""
                          [Genbox.FastEnum.FastEnum(DisableCache = {{(disableCache ? "true" : "false")}})]
                          public enum Sample
                          {
                              {{members}},
                              Alias = Value0
                          }

                          public static class Probe
                          {
                              public static bool Run()
                              {
                                  for (int i = -1; i <= 34; i++)
                                  {
                                      Sample value = (Sample)i;
                                      bool included = i >= 0 && i < 34 && i != 16;
                                      if (Enums.Sample.IsDefined(value) != included || value.TryGetUnderlyingValue(out _) != included)
                                          return false;
                                      if (i == 16 && value.GetString() != string.Empty)
                                          return false;
                                  }
                                  return true;
                              }
                          }
                          """;

        Assert.True(CompileAndRun(source));
    }

    private static bool CompileAndRun(string source)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: token);
        CSharpCompilation compilation = TestHelper.CreateCompilation([syntaxTree]).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, checkOverflow: true));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new EnumGenerator().AsSourceGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out ImmutableArray<Diagnostic> diagnostics, token);
        Assert.Empty(diagnostics);

        using MemoryStream stream = new MemoryStream();
        EmitResult emitted = output.Emit(stream, cancellationToken: token);
        Assert.True(emitted.Success, string.Join('\n', emitted.Diagnostics));

        Assembly assembly = Assembly.Load(stream.ToArray());
        MethodInfo runMethod = assembly.GetType("Probe")!.GetMethod("Run")!;
        return (bool)runMethod.Invoke(null, null)!;
    }
}