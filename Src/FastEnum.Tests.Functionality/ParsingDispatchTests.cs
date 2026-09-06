using System.Globalization;
using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class ParsingDispatchTests
{
    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    public void CharacterDispatchMatchesOrdinalComparisonForEveryChar(StringComparison comparison)
    {
        string[] names = ["K", "S", "I", "i"];
        for (int c = char.MinValue; c <= char.MaxValue; c++)
        {
            string input = ((char)c).ToString();
            int expected = Array.FindIndex(names, name => string.Equals(name, input, comparison));
            Assert.Equal(expected >= 0, Enums.AsciiDispatchEnum.TryParse(input, out var value, AsciiDispatchEnumFormat.Name, comparison));
            Assert.Equal(expected >= 0, Enums.AsciiDispatchEnum.TryParse(input.AsSpan(), out var spanValue, AsciiDispatchEnumFormat.Name, comparison));
            if (expected >= 0)
            {
                Assert.Equal((AsciiDispatchEnum)expected, value);
                Assert.Equal(value, spanValue);
            }

            // Numeric keys dispatch directly on the character without case folding.
            bool numericMatch = c is >= '0' and <= '3';
            Assert.Equal(numericMatch, Enums.AsciiDispatchEnum.TryParse(input, out value, AsciiDispatchEnumFormat.Value, comparison));
            Assert.Equal(numericMatch, Enums.AsciiDispatchEnum.TryParse(input.AsSpan(), out spanValue, AsciiDispatchEnumFormat.Value, comparison));
            if (numericMatch)
            {
                Assert.Equal(c - '0', (int)value);
                Assert.Equal(value, spanValue);
            }
        }
    }

    [Theory]
#pragma warning disable RS0030 // The API supports culture comparisons; verify their fallback semantics.
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
#pragma warning restore RS0030
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    public void CultureComparisonsRetainTheirOriginalSemantics(StringComparison comparison)
    {
        string[] names = ["coop", "co-op"];
        foreach (string input in new[] { "coop", "COOP", "co\u00adop", "co-op", "CO-OP", "missing" })
        {
            int expected = Array.FindIndex(names, name => string.Equals(input, name, comparison));
            Assert.Equal(expected >= 0, Enums.CultureDispatchEnum.TryParse(input, out var value, CultureDispatchEnumFormat.Name, comparison));
            Assert.Equal(expected >= 0, Enums.CultureDispatchEnum.TryParse(input.AsSpan(), out var spanValue, CultureDispatchEnumFormat.Name, comparison));
            if (expected >= 0)
            {
                Assert.Equal((CultureDispatchEnum)expected, value);
                Assert.Equal(value, spanValue);
            }
        }
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    public void LargeDispatchHandlesEveryMemberWithAndWithoutCaches(StringComparison comparison)
    {
        for (int i = 0; i < 130; i++)
        {
            string name = (comparison == StringComparison.Ordinal ? "Item" : "item") + i.ToString(CultureInfo.InvariantCulture);
            foreach (string input in new[] { name, i.ToString(CultureInfo.InvariantCulture) })
            {
                Assert.True(Enums.DispatchLargeEnum.TryParse(input, out var value, comparison: comparison));
                Assert.True(Enums.DispatchLargeEnum.TryParse(input.AsSpan(), out var spanValue, comparison: comparison));
                Assert.True(Enums.UncachedDispatchLargeEnum.TryParse(input, out var uncachedValue, comparison: comparison));
                Assert.True(Enums.UncachedDispatchLargeEnum.TryParse(input.AsSpan(), out var uncachedSpanValue, comparison: comparison));
                Assert.Equal(i, (int)value);
                Assert.Equal(i, (int)spanValue);
                Assert.Equal(i, (int)uncachedValue);
                Assert.Equal(i, (int)uncachedSpanValue);
            }
        }

        foreach (string input in new[] { "", "Missing", "Item130", "Item00", "Xtem129", "Item129x", "-1", "130" })
        {
            Assert.False(Enums.DispatchLargeEnum.TryParse(input, out var value, comparison: comparison));
            Assert.Equal(default, value);
            Assert.False(Enums.DispatchLargeEnum.TryParse(input.AsSpan(), out value, comparison: comparison));
            Assert.Equal(default, value);
            Assert.False(Enums.UncachedDispatchLargeEnum.TryParse(input, out var uncachedValue, comparison: comparison));
            Assert.Equal(default, uncachedValue);
            Assert.False(Enums.UncachedDispatchLargeEnum.TryParse(input.AsSpan(), out uncachedValue, comparison: comparison));
            Assert.Equal(default, uncachedValue);
        }
    }

    [Theory]
#pragma warning disable RS0030 // Exercise every supported culture comparison in the large-parser fallback.
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
#pragma warning restore RS0030
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    public void LargeCultureFallbackWorksWithAndWithoutCaches(StringComparison comparison)
    {
        (string Text, int Value)[] candidates = [("Item129", 129), ("coop", 1000), ("co-op", 1001)];
        foreach (string input in new[] { "Item129", "ITEM129", "coop", "COOP", "co\u00adop", "co-op", "CO-OP", "missing" })
        {
            int index = Array.FindIndex(candidates, candidate => string.Equals(input, candidate.Text, comparison));
            bool expected = index >= 0;
            int expectedValue = expected ? candidates[index].Value : 0;
            Assert.Equal(expected, Enums.DispatchLargeEnum.TryParse(input, out var value, DispatchLargeEnumFormat.Name, comparison));
            Assert.Equal(expectedValue, (int)value);
            Assert.Equal(expected, Enums.DispatchLargeEnum.TryParse(input.AsSpan(), out var spanValue, DispatchLargeEnumFormat.Name, comparison));
            Assert.Equal(expectedValue, (int)spanValue);
            Assert.Equal(expected, Enums.UncachedDispatchLargeEnum.TryParse(input, out var uncachedValue, UncachedDispatchLargeEnumFormat.Name, comparison));
            Assert.Equal(expectedValue, (int)uncachedValue);
            Assert.Equal(expected, Enums.UncachedDispatchLargeEnum.TryParse(input.AsSpan(), out var uncachedSpanValue, UncachedDispatchLargeEnumFormat.Name, comparison));
            Assert.Equal(expectedValue, (int)uncachedSpanValue);
        }
    }
}