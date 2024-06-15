using System.Reflection;
using Genbox.FastEnum.Tests.CodeGen.Code;
using static Genbox.FastEnum.Tests.CodeGen.Code.TestHelper;

namespace Genbox.FastEnum.Tests.CodeGen;

public class CodeGenTests
{
    [Theory]
    [MemberData(nameof(GetTests))]
    public void RunResources(string testName)
    {
        TestResource<EnumGenerator>(testName);
    }

    [Theory(Skip = "This is to update all resources with new formatting")]
    // [Theory]
    [MemberData(nameof(GetTests))]
    public void UpdateResources(string testName)
    {
        string inputSource = ReadResource(testName);
        string actual = GetGeneratedOutput<EnumGenerator>(inputSource).ReplaceLineEndings("\n");

        File.WriteAllText(@"..\..\..\Resources\" + Path.ChangeExtension(testName.Substring(40), ".output"), actual);
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