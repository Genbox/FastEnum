using Genbox.FastEnum.Tests.CodeGen.Code;

namespace Genbox.FastEnum.Tests.CodeGen;

/// <summary>
/// The tests in this class used to produce diagnostic errors. They should no longer do that, or it is a regression.
/// </summary>
public class BugTests
{
    [Theory]
    [InlineData(false, "TryGetUnderlyingValue")]
    [InlineData(true, "TryGetUnderlyingValue")]
    [InlineData(false, "All")]
    [InlineData(true, "All")]
    public void OmittedAliasCompiles(bool flags, string exclusion)
    {
        string code = $$"""
                        [FastEnum]
                        {{(flags ? "[Flags]" : "")}}
                        public enum AliasEnum
                        {
                            None = 0,
                            First = 1,
                            [EnumOmitValue(Exclude = EnumOmitExclude.{{exclusion}})]
                            Alias = First
                        }
                        """;

        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(code));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AliasMetadataCompiles(bool flags)
    {
        string code = $$"""
                        [FastEnum]
                        {{(flags ? "[Flags]" : "")}}
                        public enum AliasEnum
                        {
                            None = 0,
                            [Display(Name = "First", Description = "First description")]
                            First = 1,
                            [Display(Name = "Alias", Description = "Alias description")]
                            Alias = First
                        }
                        """;

        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(code));
    }

    [Fact]
    public void TestUlongBug()
    {
        const string code = """
                            [FastEnum]
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
        const string code = """
                            [FastEnum]
                            public enum TestEnum : long
                            {
                                None = 0,
                                Min = long.MinValue
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