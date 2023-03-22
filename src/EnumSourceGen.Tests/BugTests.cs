using Genbox.EnumSourceGen.Tests.Code;

namespace Genbox.EnumSourceGen.Tests;

public class BugTests
{
    [Fact]
    public void TestUlongBug()
    {
        string code = """
using Genbox.EnumSourceGen;

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
using Genbox.EnumSourceGen;

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