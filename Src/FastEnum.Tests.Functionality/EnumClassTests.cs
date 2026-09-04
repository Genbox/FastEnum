using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class EnumClassTests
{
    [Fact]
    public void MemberCountTest()
    {
        Assert.Equal(5, Enums.TestEnum.MemberCount);
    }

    [Fact]
    public void IsFlagEnumTest()
    {
        Assert.True(Enums.TestEnum.IsFlagEnum);
        Assert.False(Enums.NonFlagsEnum.IsFlagEnum);
    }

    [Fact]
    public void TryParseTest()
    {
        Assert.True(Enums.TestEnum.TryParse("First", out TestEnum result));
        Assert.Equal(TestEnum.First, result);

        Assert.True(Enums.TestEnum.TryParse("first", out result, TestEnumFormat.Name, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(TestEnum.First, result);
        Assert.False(Enums.TestEnum.TryParse("first", out result, TestEnumFormat.Name));

        Assert.True(Enums.TestEnum.TryParse("8", out result, TestEnumFormat.Value));
        Assert.Equal(TestEnum.First, result);

        Assert.False(Enums.TestEnum.TryParse("doesnotexist", out result));

        //Check that we also support parsing display names
        Assert.True(Enums.TestEnum.TryParse("FirstDisplayName", out result, TestEnumFormat.DisplayName));
        Assert.Equal(TestEnum.First, result);

        Assert.True(Enums.TestEnum.TryParse("FirstDescription", out result, TestEnumFormat.Description));
        Assert.Equal(TestEnum.First, result);

        Assert.False(Enums.TestEnum.TryParse("First", out result, TestEnumFormat.None));
    }

    [Fact]
    public void ParseTest()
    {
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("First"));
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("first", TestEnumFormat.Default, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("8", TestEnumFormat.Value));

        ReadOnlySpan<char> span = "First";
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse(span));

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
    public void SpanParsingHonorsFormatsAndComparison(string text, TestEnumFormat format, StringComparison comparison, bool expected)
    {
        Assert.Equal(expected, Enums.TestEnum.TryParse(text.AsSpan(), out TestEnum result, format, comparison));
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
    public void GetMemberNamesTest()
    {
        string[] names =
        [
            nameof(TestEnum.First),
            nameof(TestEnum.Second),
            nameof(TestEnum.Third),
            nameof(TestEnum.Other),
            nameof(TestEnum.Min)
        ];

        Assert.Equal(names, Enums.TestEnum.GetMemberNames());
    }

    [Fact]
    public void GetMemberValuesTest()
    {
        TestEnum[] values =
        [
            TestEnum.First,
            TestEnum.Second,
            TestEnum.Third,
            TestEnum.Other,
            TestEnum.Min
        ];

        Assert.Equal(values, Enums.TestEnum.GetMemberValues());
    }

    [Fact]
    public void GetUnderlyingValuesTest()
    {
        long[] underlyingValues =
        [
            8,
            1,
            2,
            256,
            long.MinValue
        ];

        Assert.Equal(underlyingValues, Enums.TestEnum.GetUnderlyingValues());
    }

    [Fact]
    public void GetDisplayNamesTest()
    {
        (TestEnum, string)[] displayNames =
        [
            (TestEnum.First, "FirstDisplayName")
        ];

        Assert.Equal(displayNames, Enums.TestEnum.GetDisplayNames());
    }

    [Fact]
    public void GetDescriptionsTest()
    {
        (TestEnum, string)[] descriptions =
        [
            (TestEnum.First, "FirstDescription")
        ];

        Assert.Equal(descriptions, Enums.TestEnum.GetDescriptions());
    }

    [Fact]
    public void EscapedStringsAreHandled()
    {
        Assert.True(Enums.EscapedEnum.TryParse("Val\"With\\Slash", out EscapedEnum parsed));
        Assert.Equal(EscapedEnum.Value1, parsed);

        Assert.Equal(new[] { "Val\"With\\Slash" }, Enums.EscapedEnum.GetMemberNames());
        Assert.Equal([(EscapedEnum.Value1, "C:\\Path\\File\"Name")], Enums.EscapedEnum.GetDisplayNames());
        Assert.Equal([(EscapedEnum.Value1, "Line1\\Line2")], Enums.EscapedEnum.GetDescriptions());
    }
}