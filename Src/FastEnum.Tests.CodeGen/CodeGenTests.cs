#if DEBUG

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
        string inputPath = Path.Combine(_resourcesDir, testName);
        string inputSource = await File.ReadAllTextAsync(inputPath, TestContext.Current.CancellationToken);
        string actual = GetGeneratedOutput<EnumGenerator>(inputSource);

        string fileName = Path.GetFileName(testName);
        string? category = Path.GetDirectoryName(testName);

        await Verify(actual)
              .UseFileName(fileName)
              .UseDirectory(string.IsNullOrEmpty(category) ? "Resources" : Path.Combine("Resources", category));
    }

    public static TheoryData<string> GetTests()
    {
        TheoryData<string> data = new TheoryData<string>();

        foreach (string resource in Directory.GetFiles(_resourcesDir, "*.cs", SearchOption.AllDirectories))
        {
            if (string.Equals(Path.GetFileName(resource), "_Header.cs", StringComparison.Ordinal))
                continue;

            string relativePath = Path.GetRelativePath(_resourcesDir, resource);
            data.Add(relativePath);
        }

        return data;
    }
}
#endif