using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class NullParsingTests
{
    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    public void TryParseNullReturnsFalseAndDefault(StringComparison comparison)
    {
        Assert.False(Enums.DisplayMetadataEnum.TryParse(null, out DisplayMetadataEnum directResult, DisplayMetadataEnumFormat.DisplayName, comparison));
        Assert.Equal(default, directResult);
        Assert.False(Enums.MetadataDispatchEnum.TryParse(null, out MetadataDispatchEnum treeResult, MetadataDispatchEnumFormat.Name, comparison));
        Assert.Equal(default, treeResult);
        Assert.False(Enums.DispatchLargeEnum.TryParse(null, out DispatchLargeEnum dictionaryResult, DispatchLargeEnumFormat.Name, comparison));
        Assert.Equal(default, dictionaryResult);
        Assert.False(Enums.EmptyFlags.TryParse(null, out EmptyFlags emptyResult, comparison: comparison));
        Assert.Equal(default, emptyResult);
    }

    [Fact]
    public void ParseNullThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>("value", () => Enums.MetadataDispatchEnum.Parse(null!));

    [Fact]
    public void EmptyTransformedNameRemainsParseable()
    {
        Assert.True(Enums.MetadataDispatchEnum.TryParse(string.Empty, out MetadataDispatchEnum result, MetadataDispatchEnumFormat.Name));
        Assert.Equal(MetadataDispatchEnum.Third, result);
        Assert.True(Enums.MetadataDispatchEnum.TryParse(ReadOnlySpan<char>.Empty, out result, MetadataDispatchEnumFormat.Name));
        Assert.Equal(MetadataDispatchEnum.Third, result);
    }
}