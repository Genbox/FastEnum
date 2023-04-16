using Genbox.EnumSourceGen.Tests.CodeGen.Code;

namespace Genbox.EnumSourceGen.Tests.CodeGen;

/// <summary>
/// The tests in this class used to produce diagnostic errors. They should no longer do that, or it is a regression.
/// </summary>
public class BugTests
{
    [Fact]
    public void TestUlongBug()
    {
        string code = """
[EnumSourceGen]
public enum TestEnum : ulong
{
    None = 0,
    Max = ulong.MaxValue
}
""";
        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(code));
    }

    [Fact]
    public void TestNegativeValueBug()
    {
        string code = """
[EnumSourceGen]
public enum TestEnum : long
{
    None = 0,
    Min = long.MinValue
}
""";

        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(code));
    }
}