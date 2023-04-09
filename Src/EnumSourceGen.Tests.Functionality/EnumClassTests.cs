using Genbox.EnumSourceGen.Tests.Functionality.Code;

namespace Genbox.EnumSourceGen.Tests.Functionality;

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
    }

    [Fact]
    public void TryParseTest()
    {
        Assert.True(Enums.TestEnum.TryParse("First", out TestEnum result));
        Assert.Equal(TestEnum.First, result);

        Assert.True(Enums.TestEnum.TryParse("first", out result, TestEnumFormat.Name, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(TestEnum.First, result);

        //Check if we support span inputs
        ReadOnlySpan<char> span = "first";
        Assert.True(Enums.TestEnum.TryParse(span, out result, TestEnumFormat.Name, StringComparison.OrdinalIgnoreCase));

        Assert.False(Enums.TestEnum.TryParse("doesnotexist", out result));

        //Check that we also support parsing display names
        Assert.True(Enums.TestEnum.TryParse("FirstDisplayName", out result, TestEnumFormat.DisplayName));
        Assert.Equal(TestEnum.First, result);
    }

    [Fact]
    public void ParseTest()
    {
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("First"));
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("first", TestEnumFormat.Default, StringComparison.OrdinalIgnoreCase));
        Assert.Throws<ArgumentOutOfRangeException>(() => Enums.TestEnum.Parse("doesnotexist"));
    }

    [Fact]
    public void IsDefinedTest()
    {
        //Test flag combinations
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second | TestEnum.Third | TestEnum.Other | TestEnum.Min));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second | TestEnum.Third | TestEnum.Other));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second | TestEnum.Third));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First));
        Assert.False(Enums.TestEnum.IsDefined((TestEnum)100));

        //We also explicitly test a non flags enum
        Assert.True(Enums.NonFlagsEnum.IsDefined(NonFlagsEnum.Value1));
        Assert.True(Enums.NonFlagsEnum.IsDefined(NonFlagsEnum.Value2));
        Assert.False(Enums.NonFlagsEnum.IsDefined((NonFlagsEnum)48));
    }

    [Fact]
    public void GetMemberNamesTest()
    {
        string[] names =
        {
            nameof(TestEnum.First),
            nameof(TestEnum.Second),
            nameof(TestEnum.Third),
            nameof(TestEnum.Other),
            nameof(TestEnum.Min)
        };

        Assert.Equal(names, Enums.TestEnum.GetMemberNames());
    }

    [Fact]
    public void GetMemberValuesTest()
    {
        TestEnum[] values =
        {
            TestEnum.First,
            TestEnum.Second,
            TestEnum.Third,
            TestEnum.Other,
            TestEnum.Min
        };

        Assert.Equal(values, Enums.TestEnum.GetMemberValues());
    }

    [Fact]
    public void GetUnderlyingValuesTest()
    {
        long[] underlyingValues =
        {
            8,
            1,
            2,
            256,
            long.MinValue
        };

        Assert.Equal(underlyingValues, Enums.TestEnum.GetUnderlyingValues());
    }

    [Fact]
    public void GetDisplayNamesTest()
    {
        (TestEnum, string)[] displayNames =
        {
            (TestEnum.First, "FirstDisplayName")
        };

        Assert.Equal(displayNames, Enums.TestEnum.GetDisplayNames());
    }

    [Fact]
    public void GetDescriptionsTest()
    {
        (TestEnum, string)[] descriptions =
        {
            (TestEnum.First, "FirstDescription")
        };

        Assert.Equal(descriptions, Enums.TestEnum.GetDescriptions());
    }
}