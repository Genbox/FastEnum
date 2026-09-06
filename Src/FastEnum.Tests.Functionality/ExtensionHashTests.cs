using System.Globalization;
using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class ExtensionHashTests
{
    [Fact]
    public void LookupsPreserveNamesMetadataAliasesAndOmissions()
    {
        for (int i = 0; i < 40; i++)
        {
            ExtensionHashEnum value = (ExtensionHashEnum)(i * 256L);
            string suffix = i.ToString(CultureInfo.InvariantCulture);
            string name = i switch { 0 => "Alias", 1 => string.Empty, _ => "Value" + suffix };
            Assert.Equal(name, value.GetString());
            Assert.Equal(name, value.GetString(ExtensionHashEnumFormat.Name));
            string displayText = i switch { 0 => "Excluded alias", 1 => string.Empty, 2 => name, _ => "Label" + suffix };
            string descriptionText = i switch { 0 => "Excluded alias", 1 => string.Empty, 2 => name, _ => "Detail" + suffix };
            Assert.Equal(displayText, value.GetString(ExtensionHashEnumFormat.DisplayName));
            Assert.Equal(descriptionText, value.GetString(ExtensionHashEnumFormat.Description));

            Assert.Equal(i != 1, value.TryGetUnderlyingValue(out long underlying));
            Assert.Equal(i == 1 ? 0 : i * 256L, underlying);
            if (i != 1)
                Assert.Equal(i * 256L, value.GetUnderlyingValue());
            else
                Assert.Throws<ArgumentOutOfRangeException>(() => value.GetUnderlyingValue());

            bool hasMetadata = i is not (1 or 2);
            Assert.Equal(hasMetadata, value.TryGetDisplayName(out string? display));
            Assert.Equal(hasMetadata, value.TryGetDescription(out string? description));
            Assert.Equal(hasMetadata ? "Label" + suffix : null, display);
            Assert.Equal(hasMetadata ? "Detail" + suffix : null, description);

            if (hasMetadata)
            {
                Assert.Equal(display, value.GetDisplayName());
                Assert.Equal(description, value.GetDescription());
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => value.GetDisplayName());
                Assert.Throws<ArgumentOutOfRangeException>(() => value.GetDescription());
            }
        }

        // Formatting and metadata lookup have different omission rules for the first alias.
        Assert.Equal("Excluded alias", ExtensionHashEnum.Alias.GetString(ExtensionHashEnumFormat.DisplayName));
        Assert.Equal("Excluded alias", ExtensionHashEnum.Alias.GetString(ExtensionHashEnumFormat.Description));
        Assert.Equal(string.Empty, ExtensionHashEnum.Value1.GetString(ExtensionHashEnumFormat.DisplayName));
        Assert.Equal(string.Empty, ExtensionHashEnum.Value1.GetString(ExtensionHashEnumFormat.Description));
        Assert.Equal("Value2", ExtensionHashEnum.Value2.GetString(ExtensionHashEnumFormat.DisplayName | ExtensionHashEnumFormat.Name));
    }

    [Fact]
    public void LookupMissesNeverReturnAnotherBucketsValue()
    {
        // Neighbours of the shifted keys exercise misses in populated hash buckets.
        long[] misses = Enumerable.Range(0, 40).Select(i => (i * 256L) + 1)
                                  .Concat([long.MinValue, long.MaxValue, -1L]).ToArray();

        foreach (long raw in misses)
        {
            ExtensionHashEnum value = (ExtensionHashEnum)raw;
            string numeric = raw.ToString(CultureInfo.InvariantCulture);
            Assert.Equal(numeric, value.GetString());
            Assert.Equal(numeric, value.GetString(ExtensionHashEnumFormat.Name));
            Assert.Equal(numeric, value.GetString(ExtensionHashEnumFormat.DisplayName | ExtensionHashEnumFormat.Description));
            Assert.False(value.TryGetUnderlyingValue(out long underlying));
            Assert.Equal(0, underlying);
            Assert.False(value.TryGetDisplayName(out string? display));
            Assert.Null(display);
            Assert.False(value.TryGetDescription(out string? description));
            Assert.Null(description);
            Assert.Throws<ArgumentOutOfRangeException>(() => value.GetUnderlyingValue());
            Assert.Throws<ArgumentOutOfRangeException>(() => value.GetDisplayName());
            Assert.Throws<ArgumentOutOfRangeException>(() => value.GetDescription());
        }
    }
}