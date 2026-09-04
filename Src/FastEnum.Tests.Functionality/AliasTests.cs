using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class AliasTests
{
    [Fact]
    public void AliasesHonorEnumTransform()
    {
        Assert.Equal("first", TransformedAliasEnum.First.GetString());
        Assert.Equal("first", TransformedAliasEnum.First.GetString(TransformedAliasEnumFormat.Name));
        Assert.True(Enums.TransformedAliasEnum.TryParse(TransformedAliasEnum.First.GetString(), out TransformedAliasEnum result));
        Assert.Equal(TransformedAliasEnum.First, result);
    }

    [Theory]
    [InlineData((int)AliasEnum.First, "First name", "First description")]
    [InlineData((int)AliasEnum.IncludedMetadata, "Included name", "Included description")]
    [InlineData((int)AliasEnum.AvailableMetadata, "Available name", "Available description")]
    public void MetadataUsesFirstIncludedAlias(int rawValue, string name, string description)
    {
        AliasEnum value = (AliasEnum)rawValue;
        Assert.Equal(name, value.GetDisplayName());
        Assert.Equal(description, value.GetDescription());
    }

    [Theory]
    [InlineData((int)OmittedFormattingEnum.OmitAll)]
    [InlineData((int)OmittedFormattingEnum.OmitString)]
    [InlineData((int)OmittedFormattingEnum.WithoutMetadata)]
    public void FormattingHonorsOmission(int rawValue)
    {
        OmittedFormattingEnum value = (OmittedFormattingEnum)rawValue;
        Assert.Equal(string.Empty, value.GetString(OmittedFormattingEnumFormat.DisplayName));
        Assert.Equal(string.Empty, value.GetString(OmittedFormattingEnumFormat.Description));
        Assert.Equal(string.Empty, value.GetString(OmittedFormattingEnumFormat.Name));
        Assert.Equal(string.Empty, value.GetString(OmittedFormattingEnumFormat.Value));
    }

    [Fact]
    public void FormattingOmissionDoesNotOmitMetadataLookup()
    {
        Assert.Equal("Retained name", OmittedFormattingEnum.OmitString.GetDisplayName());
        Assert.Equal("Retained description", OmittedFormattingEnum.OmitString.GetDescription());
        Assert.False(OmittedFormattingEnum.OmitAll.TryGetDisplayName(out _));
        Assert.False(OmittedFormattingEnum.OmitAll.TryGetDescription(out _));
    }
}