using System.Collections.Immutable;
using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Genbox.FastEnum.Tests.CodeGen;

public class GeneratedCodeTests
{
    [Theory]
    [InlineData("NETSTANDARD2_1_OR_GREATER")]
    [InlineData("NETCOREAPP3_1_OR_GREATER")]
    public void SpanParsingCompilesWithoutGlobalUsings(string frameworkSymbol)
    {
        const string code = """
                            [FastEnum]
                            [Flags]
                            [EnumTransform(Preset = EnumTransform.LowerCase)]
                            public enum SpanEnum
                            {
                                None = 0,
                                [Display(Name = "First display", Description = "First description")]
                                First = 1,
                                [EnumOmitValue(Exclude = EnumOmitExclude.TryParse)]
                                Second = 2
                            }
                            """;

        CSharpParseOptions options = new CSharpParseOptions(preprocessorSymbols: [frameworkSymbol]);
        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> generatorDiagnostics, out IEnumerable<Diagnostic> compilerDiagnostics, options);
        Assert.Empty(generatorDiagnostics);
        Assert.Empty(compilerDiagnostics);
    }

    [Fact]
    public void AttributeContainsNameAndVersion()
    {
        const string code = """
                            [FastEnum]
                            public enum TestEnum
                            {
                                Value
                            }
                            """;

        string output = TestHelper.GetGeneratedOutput<EnumGenerator>(code);
        string version = typeof(EnumGenerator).Assembly.GetName().Version!.ToString();
        string attribute = $"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"FastEnum\", \"{version}\")]";

        Assert.Equal(4, output.Split(attribute).Length - 1);
    }
}