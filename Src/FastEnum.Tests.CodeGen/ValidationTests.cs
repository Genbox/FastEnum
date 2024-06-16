using System.Collections.Immutable;
using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;

namespace Genbox.FastEnum.Tests.CodeGen;

public class ValidationTests
{
    [Fact]
    public void TestLessVisibleClass()
    {
        string code = """
internal class MyInternalClass
{
    [FastEnum]
    public enum MyPublicEnum1
    {
        Value
    }
}
""";
        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out _);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Equal("FE001", res.Id); //Enum visibility validation failed
    }
}