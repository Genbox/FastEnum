#if DEBUG

using System.Reflection;
using Genbox.FastEnum.Tests.CodeGen.Code;
using static Genbox.FastEnum.Tests.CodeGen.Code.TestHelper;

namespace Genbox.FastEnum.Tests.CodeGen;

public class CodeGenTests
{
    private static readonly string _resourcesDir = AppContext.BaseDirectory + "../../../Resources";

    [Fact]
    public Task VerifyChecksTest() => VerifyChecks.Run();

    [Theory]
    [MemberData(nameof(GetTests))]
    public async Task RunResources(string testName)
    {
        string inputSource = await File.ReadAllTextAsync(Path.Combine(_resourcesDir, testName));
        string actual = GetGeneratedOutput<EnumGenerator>(inputSource);

        await Verify(actual)
              .UseFileName(testName)
              .UseDirectory("Resources")
              .DisableDiff();
    }

    public static TheoryData<string> GetTests()
    {
        TheoryData<string> data = new TheoryData<string>();

        foreach (string resource in Directory.GetFiles(_resourcesDir))
        {
            if (resource.EndsWith(".input", StringComparison.OrdinalIgnoreCase))
                data.Add(Path.GetFileName(resource));
        }

        return data;
    }
}
#endif