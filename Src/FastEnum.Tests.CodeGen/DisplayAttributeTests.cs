using Genbox.FastEnum.Tests.CodeGen.Code;

namespace Genbox.FastEnum.Tests.CodeGen;

public class DisplayAttributeTests
{
    [Theory]
    [InlineData("Order = 1")]
    [InlineData("ShortName = \"Short\"")]
    [InlineData("GroupName = \"Group\"")]
    [InlineData("Prompt = \"Prompt\"")]
    [InlineData("AutoGenerateField = false")]
    [InlineData("AutoGenerateFilter = true")]
    [InlineData("ResourceType = typeof(DisplayResources)")]
    [InlineData("ShortName = null")]
    public void UnmappedPropertiesDoNotPreventGeneration(string property)
    {
        string source = $$"""
            public static class DisplayResources
            {
                public static string Label => "Label";
                public static string Details => "Details";
            }

            [FastEnum]
            public enum DisplayEnum
            {
                [Display(Name = "Label", Description = "Details", {{property}})]
                None
            }

            [FastEnum]
            public enum OtherEnum { None }
            """;

        string output = TestHelper.GetGeneratedOutput<EnumGenerator>(source);
        Assert.Contains("displayName = \"Label\";", output, StringComparison.Ordinal);
        Assert.Contains("description = \"Details\";", output, StringComparison.Ordinal);
        Assert.Contains("class OtherEnumExtensions", output, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("\\u0085")]
    [InlineData("\\u2028")]
    [InlineData("\\u2029")]
    public void UnicodeNewlinesInDisplayTextCompile(string escape)
    {
        string source = $$"""
            [FastEnum]
            public enum DisplayEnum
            {
                [Display(Name = "First{{escape}}Second", Description = "Third{{escape}}Fourth")]
                None
            }
            """;

        Assert.NotEmpty(TestHelper.GetGeneratedOutput<EnumGenerator>(source));
    }
}