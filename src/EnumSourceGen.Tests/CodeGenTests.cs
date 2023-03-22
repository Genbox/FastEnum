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