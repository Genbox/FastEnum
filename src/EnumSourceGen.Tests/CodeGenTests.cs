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

    [Theory(Skip = "This is to update all tests with new formatting")]
    // [Theory]
    [MemberData(nameof(GetTests))]
    public void UpdateResources(string testName)
    {
        string inputSource = TestHelper.ReadResource(testName);
        string actual = TestHelper.GetGeneratedOutput<EnumGenerator>(inputSource).ReplaceLineEndings("\n");

        File.WriteAllText(@"..\..\..\Resources\" + Path.ChangeExtension(testName.Substring(37), ".output"), actual);
    }
}