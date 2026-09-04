using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class EnumExtensionsTest
{
    private const TestEnum _valid = TestEnum.First;
    private const TestEnum _invalid = (TestEnum)100;

    [Fact]
    public void GetStringTest()
    {
        Assert.Equal("First", _valid.GetString());
    }

    [Fact]
    public void GetUnderlyingValueTest()
    {
        Assert.True(_valid.TryGetUnderlyingValue(out long underlyingValue));
        Assert.Equal(8, underlyingValue);

        Assert.False(_invalid.TryGetUnderlyingValue(out underlyingValue));

        Assert.Equal(8, _valid.GetUnderlyingValue());
        Assert.Throws<ArgumentOutOfRangeException>(() => _invalid.GetUnderlyingValue());
    }

    [Fact]
    public void GetUnderlyingValueSupportsFlagCombinations()
    {
        const TestEnum combined = TestEnum.Second | TestEnum.Third;

        Assert.True(combined.TryGetUnderlyingValue(out long underlyingValue));
        Assert.Equal(3, underlyingValue);
        Assert.Equal(3, combined.GetUnderlyingValue());
    }

    [Fact]
    public void GetUnderlyingValueHonorsOmittedCompositeFlags()
    {
        Assert.False(OmittedCompositeFlagsEnum.Both.TryGetUnderlyingValue(out _));
        Assert.Throws<ArgumentOutOfRangeException>(() => OmittedCompositeFlagsEnum.Both.GetUnderlyingValue());
    }

    [Fact]
    public void GetDisplayNameTest()
    {
        Assert.True(_valid.TryGetDisplayName(out string? displayName));
        Assert.Equal("FirstDisplayName", displayName);

        Assert.False(_invalid.TryGetDisplayName(out displayName));

        Assert.Equal("FirstDisplayName", _valid.GetDisplayName());
        Assert.Throws<ArgumentOutOfRangeException>(() => _invalid.GetDisplayName());
    }

    [Fact]
    public void GetDescriptionTest()
    {
        Assert.True(_valid.TryGetDescription(out string? description));
        Assert.Equal("FirstDescription", description);

        Assert.False(_invalid.TryGetDescription(out description));

        Assert.Equal("FirstDescription", _valid.GetDescription());
        Assert.Throws<ArgumentOutOfRangeException>(() => _invalid.GetDescription());
    }

    [Theory]
    [InlineData(TestEnum.First, TestEnum.First, true)]
    [InlineData(TestEnum.Second, TestEnum.First, false)]
    [InlineData(TestEnum.First | TestEnum.Second, TestEnum.First, true)]
    [InlineData(TestEnum.First, TestEnum.First | TestEnum.Second, false)]
    [InlineData(TestEnum.First | TestEnum.Second, TestEnum.First | TestEnum.Second, true)]
    [InlineData(TestEnum.First, (TestEnum)0, true)]
    [InlineData(TestEnum.Min, TestEnum.Min, true)]
    [InlineData((TestEnum)32, (TestEnum)32, true)]
    public void IsFlagSetTest(TestEnum value, TestEnum flag, bool expected)
    {
        // Cover zero, partial composites, the sign bit, and unnamed values explicitly.
        Assert.Equal(expected, value.IsFlagSet(flag));
    }

    [Fact]
    public void EscapedStringsAreReturnedAndParsed()
    {
        const EscapedEnum value = EscapedEnum.Value1;

        Assert.Equal("Val\"With\\Slash", value.GetString());

        Assert.True(value.TryGetDisplayName(out string? displayName));
        Assert.Equal("C:\\Path\\File\"Name", displayName);

        Assert.True(value.TryGetDescription(out string? description));
        Assert.Equal("Line1\\Line2", description);
    }

    [Fact]
    public void GetStringHonorsFormat()
    {
        Assert.Equal("FirstDisplayName", TestEnum.First.GetString(TestEnumFormat.DisplayName));
        Assert.Equal("FirstDescription", TestEnum.First.GetString(TestEnumFormat.Description));
        Assert.Equal("First", TestEnum.First.GetString(TestEnumFormat.Name));
        Assert.Equal("8", TestEnum.First.GetString(TestEnumFormat.Value));
    }
}