using Genbox.FastEnum.Tests.CodeGen.Code;

namespace Genbox.FastEnum.Tests.CodeGen;

/// <summary>
/// The tests in this class used to produce diagnostic errors. They should no longer do that, or it is a regression.
/// </summary>
public class BugTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OmittedAliasCompiles(bool flags)
    {
        string code = $$"""
                        [FastEnum]
                        {{(flags ? "[Flags]" : "")}}
                        public enum AliasEnum
                        {
                            None = 0,
                            First = 1,
                            [EnumOmitValue(Exclude = EnumOmitExclude.TryGetUnderlyingValue)]
                            Alias = First
                        }
                        """;

        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(code));
    }

    [Fact]
    public async Task TestIssue3()
    {
        const string code = """
                            [FastEnum(EnumsClassVisibility = Visibility.Internal, ExtensionClassVisibility = Visibility.Internal)]
                            public enum TestEnum
                            {
                                None = 0,
                                Value
                            }
                            """;

        await Verify(TestHelper.GetGeneratedOutput<EnumGenerator>(code))
              .UseFileName(nameof(TestIssue3))
              .UseDirectory("Issues");
    }
}