using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class MetadataParsingTests
{
    [Theory]
    [InlineData("Shared", MetadataDispatchEnumFormat.DisplayName, StringComparison.Ordinal, MetadataDispatchEnum.First)]
    [InlineData("SHARED", MetadataDispatchEnumFormat.DisplayName, StringComparison.Ordinal, MetadataDispatchEnum.Fourth)]
    [InlineData("shared", MetadataDispatchEnumFormat.DisplayName, StringComparison.OrdinalIgnoreCase, MetadataDispatchEnum.First)]
    [InlineData("Details", MetadataDispatchEnumFormat.Description, StringComparison.Ordinal, MetadataDispatchEnum.First)]
    [InlineData("DETAILS", MetadataDispatchEnumFormat.Description, StringComparison.Ordinal, MetadataDispatchEnum.Fourth)]
    [InlineData("details", MetadataDispatchEnumFormat.Description, StringComparison.OrdinalIgnoreCase, MetadataDispatchEnum.First)]
    [InlineData("Shadow", MetadataDispatchEnumFormat.DisplayName, StringComparison.Ordinal, MetadataDispatchEnum.Fifth)]
    [InlineData("shadow", MetadataDispatchEnumFormat.DisplayName, StringComparison.OrdinalIgnoreCase, MetadataDispatchEnum.Fifth)]
    [InlineData("Detourx", MetadataDispatchEnumFormat.Description, StringComparison.Ordinal, MetadataDispatchEnum.Fifth)]
    [InlineData("detourx", MetadataDispatchEnumFormat.Description, StringComparison.OrdinalIgnoreCase, MetadataDispatchEnum.Fifth)]
    [InlineData("2", MetadataDispatchEnumFormat.Default, StringComparison.Ordinal, MetadataDispatchEnum.First)]
    [InlineData("2", MetadataDispatchEnumFormat.Value, StringComparison.Ordinal, MetadataDispatchEnum.Second)]
    [InlineData("2", MetadataDispatchEnumFormat.DisplayName, StringComparison.Ordinal, MetadataDispatchEnum.Third)]
    [InlineData("2", MetadataDispatchEnumFormat.Value | MetadataDispatchEnumFormat.DisplayName, StringComparison.Ordinal, MetadataDispatchEnum.Second)]
    [InlineData("Shared", MetadataDispatchEnumFormat.DisplayName | MetadataDispatchEnumFormat.Description, StringComparison.Ordinal, MetadataDispatchEnum.First)]
    [InlineData("Shared", MetadataDispatchEnumFormat.Description, StringComparison.Ordinal, MetadataDispatchEnum.Third)]
    [InlineData("", MetadataDispatchEnumFormat.Name, StringComparison.Ordinal, MetadataDispatchEnum.Third)]
    [InlineData("", MetadataDispatchEnumFormat.Default, StringComparison.OrdinalIgnoreCase, MetadataDispatchEnum.Third)]
    public void ParsingPreservesDeclarationOrderAndFormatPrecedence(string input, MetadataDispatchEnumFormat format,
        StringComparison comparison, MetadataDispatchEnum expected)
    {
        Assert.True(Enums.MetadataDispatchEnum.TryParse(input, out var result, format, comparison));
        Assert.Equal(expected, result);
        Assert.True(Enums.MetadataDispatchEnum.TryParse(input.AsSpan(), out var spanResult, format, comparison));
        Assert.Equal(expected, spanResult);
    }

    [Theory]
    [InlineData("shared", MetadataDispatchEnumFormat.DisplayName)]
    [InlineData("details", MetadataDispatchEnumFormat.Description)]
    [InlineData("Shored", MetadataDispatchEnumFormat.DisplayName)]
    [InlineData("Detailx", MetadataDispatchEnumFormat.Description)]
    [InlineData("Omitted", MetadataDispatchEnumFormat.Default)]
    [InlineData("5", MetadataDispatchEnumFormat.Default)]
    [InlineData("", MetadataDispatchEnumFormat.Value)]
    [InlineData("", MetadataDispatchEnumFormat.None)]
    public void ParsingMissesResetTheResult(string input, MetadataDispatchEnumFormat format)
    {
        Assert.False(Enums.MetadataDispatchEnum.TryParse(input, out var result, format));
        Assert.Equal(default, result);
        Assert.False(Enums.MetadataDispatchEnum.TryParse(input.AsSpan(), out result, format));
        Assert.Equal(default, result);
    }
}