using System.Reflection;
using Genbox.EnumSourceGen.Tests.Code;

namespace Genbox.EnumSourceGen.Tests;

public class CodeGenTests
{
    [Theory]
    [MemberData(nameof(GetTests))]
    public void TestAll(string testName)
    {
        TestHelper.TestResource<EnumGenerator>(testName);
    }

    [Fact]
    public void TestUlongBug()
    {
        string code = """
using Genbox.EnumSourceGen;

[EnumSourceGen]
public enum BigValueEnum : ulong
{
    None = 0,
    Max = ulong.MaxValue
}
""";

        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(code));
    }


    public static IEnumerable<object[]> GetTests()
    {
        Assembly assembly = typeof(TestHelper).Assembly;
        string[] resources = assembly.GetManifestResourceNames();

        foreach (string resource in resources)
        {
            if (resource.Contains(".input", StringComparison.OrdinalIgnoreCase))
                yield return new object[] { resource };
        }
    }
}