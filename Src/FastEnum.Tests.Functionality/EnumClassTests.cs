using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class EnumClassTests
{
    [Fact]
    public void MemberCountTest() => Assert.Equal(5, Enums.TestEnum.MemberCount);

    [Fact]
    public void IsFlagEnumTest()
    {
        Assert.True(Enums.TestEnum.IsFlagEnum);
        Assert.False(Enums.NonFlagsEnum.IsFlagEnum);
    }

    [Fact]
    public void TryParseUsesDefaultFormatAndComparison()
    {
        Assert.True(Enums.TestEnum.TryParse("First", out TestEnum result));
        Assert.Equal(TestEnum.First, result);
        Assert.True(Enums.TestEnum.TryParse("First".AsSpan(), out result));
        Assert.Equal(TestEnum.First, result);
        Assert.False(Enums.TestEnum.TryParse("missing", out result));
        Assert.Equal(default, result);
        Assert.False(Enums.TestEnum.TryParse("missing".AsSpan(), out result));
        Assert.Equal(default, result);
        Assert.False(Enums.TestEnum.TryParse("first", out result, TestEnumFormat.Name));
        Assert.Equal(default, result);
        Assert.False(Enums.TestEnum.TryParse("first".AsSpan(), out result, TestEnumFormat.Name));
        Assert.Equal(default, result);
    }

    [Fact]
    public void ParseTest()
    {
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("First"));
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("first", TestEnumFormat.Default, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("8", TestEnumFormat.Value));

        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("First".AsSpan()));

        Assert.Throws<ArgumentOutOfRangeException>(() => Enums.TestEnum.Parse("doesnotexist"));
    }

    [Theory]
    [InlineData("First", TestEnumFormat.Name, StringComparison.Ordinal, true)]
    [InlineData("first", TestEnumFormat.Name, StringComparison.Ordinal, false)]
    [InlineData("first", TestEnumFormat.Name, StringComparison.OrdinalIgnoreCase, true)]
    [InlineData("8", TestEnumFormat.Value, StringComparison.Ordinal, true)]
    [InlineData("FirstDisplayName", TestEnumFormat.DisplayName, StringComparison.Ordinal, true)]
    [InlineData("FirstDescription", TestEnumFormat.Description, StringComparison.Ordinal, true)]
    [InlineData("missing", TestEnumFormat.Default, StringComparison.Ordinal, false)]
    [InlineData("First", TestEnumFormat.None, StringComparison.Ordinal, false)]
    public void ParsingHonorsFormatsAndComparison(string text, TestEnumFormat format, StringComparison comparison, bool expected)
    {
        Assert.Equal(expected, Enums.TestEnum.TryParse(text, out TestEnum result, format, comparison));
        Assert.Equal(expected ? TestEnum.First : default, result);
        Assert.Equal(expected, Enums.TestEnum.TryParse(text.AsSpan(), out result, format, comparison));
        Assert.Equal(expected ? TestEnum.First : default, result);
    }

    [Fact]
    public void IsDefinedTest()
    {
        //Test flag combinations
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second | TestEnum.Third | TestEnum.Other | TestEnum.Min));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First));
        Assert.False(Enums.TestEnum.IsDefined((TestEnum)100));

        //We also explicitly test a non flags enum
        Assert.True(Enums.NonFlagsEnum.IsDefined(NonFlagsEnum.Value1));
        Assert.False(Enums.NonFlagsEnum.IsDefined((NonFlagsEnum)48));
    }

    [Fact]
    public void GetMemberNamesTest() => Assert.Equal([nameof(TestEnum.First), nameof(TestEnum.Second), nameof(TestEnum.Third), nameof(TestEnum.Other), nameof(TestEnum.Min)], Enums.TestEnum.GetMemberNames());

    [Fact]
    public void GetMemberValuesTest() => Assert.Equal([TestEnum.First, TestEnum.Second, TestEnum.Third, TestEnum.Other, TestEnum.Min], Enums.TestEnum.GetMemberValues());

    [Fact]
    public void GetUnderlyingValuesTest() => Assert.Equal([8, 1, 2, 256, long.MinValue], Enums.TestEnum.GetUnderlyingValues());

    [Fact]
    public void GetDisplayNamesTest() => Assert.Equal([(TestEnum.First, "FirstDisplayName")], Enums.TestEnum.GetDisplayNames());

    [Fact]
    public void GetDescriptionsTest() => Assert.Equal([(TestEnum.First, "FirstDescription")], Enums.TestEnum.GetDescriptions());

    [Fact]
    public void EscapedStringsAreHandled()
    {
        Assert.True(Enums.EscapedEnum.TryParse("Val\"With\\Slash", out EscapedEnum parsed));
        Assert.Equal(EscapedEnum.Value1, parsed);

        Assert.Equal(["Val\"With\\Slash"], Enums.EscapedEnum.GetMemberNames());
        Assert.Equal([(EscapedEnum.Value1, "C:\\Path\\File\"Name")], Enums.EscapedEnum.GetDisplayNames());
        Assert.Equal([(EscapedEnum.Value1, "Line1\\Line2")], Enums.EscapedEnum.GetDescriptions());
    }
}