using Genbox.FastEnum.Tests.CodeGen.Code;

namespace Genbox.FastEnum.Tests.CodeGen;

public class DisplayAttributeTests
{
    [Fact]
    public void UnmappedResourceAndNullPropertiesDoNotPreventGeneration()
    {
        const string source = """
            public static class DisplayResources
            {
                public static string Label => "Label";
                public static string Details => "Details";
            }

            [FastEnum]
            public enum DisplayEnum
            {
                [Display(Name = "Label", Description = "Details", ResourceType = typeof(DisplayResources), ShortName = null)]
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
}