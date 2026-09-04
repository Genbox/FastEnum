using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class DisplayAttributeTests
{
    [Fact]
    public void DisplayTextIsPreservedAcrossGeneratedApis()
    {
        const string name = "First\u0085Second\u2028Third\u2029Fourth";
        const string description = "Details\u2029Next";
        const DisplayMetadataEnum value = DisplayMetadataEnum.None;

        Assert.Equal(name, value.GetDisplayName());
        Assert.Equal(description, value.GetDescription());
        Assert.Equal(name, value.GetString(DisplayMetadataEnumFormat.DisplayName));
        Assert.Equal(description, value.GetString(DisplayMetadataEnumFormat.Description));
        Assert.Equal([(value, name)], Enums.DisplayMetadataEnum.GetDisplayNames());
        Assert.Equal([(value, description)], Enums.DisplayMetadataEnum.GetDescriptions());
        Assert.Equal(value, Enums.DisplayMetadataEnum.Parse(name, DisplayMetadataEnumFormat.DisplayName));
        Assert.Equal(value, Enums.DisplayMetadataEnum.Parse(description, DisplayMetadataEnumFormat.Description));
        Assert.Equal(value, Enums.DisplayMetadataEnum.Parse(name.AsSpan(), DisplayMetadataEnumFormat.DisplayName));
        Assert.Equal(value, Enums.DisplayMetadataEnum.Parse(description.AsSpan(), DisplayMetadataEnumFormat.Description));
        Assert.False(Enums.DisplayMetadataEnum.TryParse("Short", out _, DisplayMetadataEnumFormat.DisplayName));
    }
}