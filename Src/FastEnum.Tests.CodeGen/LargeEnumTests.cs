using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Genbox.FastEnum.Tests.CodeGen;

public class LargeEnumTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LargeEnumWithOmissionsCompiles(bool disableCache)
    {
        string members = string.Join(",\n", Enumerable.Range(0, 1024).Select(i => $"Value{i} = {i * 2}"));
        string code = $$"""
                        [FastEnum(DisableCache = {{(disableCache ? "true" : "false")}})]
                        public enum LargeEnum
                        {
                            [EnumOmitValue(Exclude = EnumOmitExclude.IsDefined)]
                            Omitted = -1,
                            {{members}}
                        }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code);
    }

    [Theory]
    [InlineData(128, false)]
    [InlineData(129, true)]
    [InlineData(1024, true)]
    public async Task IsDefinedLimitsSwitchSize(int memberCount, bool usesLoop)
    {
        string members = string.Join(",\n", Enumerable.Range(0, memberCount).Select(i => $"Value{i} = {i * 2}"));
        string code = $$"""
                        [FastEnum]
                        public enum LargeEnum
                        {
                            {{members}}
                        }
                        """;

        string output = TestHelper.GetGeneratedOutput<EnumGenerator>(code);
        SyntaxNode root = await CSharpSyntaxTree.ParseText(output, cancellationToken: TestContext.Current.CancellationToken)
                                               .GetRootAsync(TestContext.Current.CancellationToken);
        MethodDeclarationSyntax method = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                             .Single(x => x.Identifier.ValueText == "IsDefined");

        Assert.Equal(usesLoop, method.DescendantNodes().OfType<ForStatementSyntax>().Any());
        Assert.Equal(!usesLoop, method.DescendantNodes().OfType<SwitchStatementSyntax>().Any());
        await Verify(method.NormalizeWhitespace().ToFullString()).UseParameters(memberCount, usesLoop).UseDirectory("Snapshots");
    }
}