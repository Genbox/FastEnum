using Genbox.EnumSourceGen.Tests.Code;

namespace Genbox.EnumSourceGen.Tests;

public class FunctionalityTests
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
        Assert.True(_valid.TryGetUnderlyingValue(out int underlyingValue));
        Assert.Equal(8, underlyingValue);

        Assert.False(_invalid.TryGetUnderlyingValue(out underlyingValue));

        Assert.Equal(8, _valid.GetUnderlyingValue());
        Assert.Throws<ArgumentOutOfRangeException>(() => _invalid.GetUnderlyingValue());
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

    [Fact]
    public void IsFlagSetTest()
    {
        TestEnum e = TestEnum.First;
        Assert.True(e.IsFlagSet(TestEnum.First));

        e = TestEnum.Second;
        Assert.False(e.IsFlagSet(TestEnum.First));

        e = TestEnum.First | TestEnum.Second | TestEnum.Third;
        Assert.True(e.IsFlagSet(TestEnum.First));
        Assert.True(e.IsFlagSet(TestEnum.Second));
        Assert.True(e.IsFlagSet(TestEnum.Third));

        //Exhaustive test against dotnet's HasFlag(). 8 is the highest value of TestEnum.
        for (int i = 0; i < 256; i++)
        {
            TestEnum value = (TestEnum)i;

            for (int j = 0; j < 8; j++)
            {
                TestEnum flags = (TestEnum)j;

                Assert.Equal(value.HasFlag(flags), value.IsFlagSet(flags));
            }
        }
    }

    [Fact]
    public void MemberCountTest()
    {
        Assert.Equal(4, Enums.TestEnum.MemberCount);
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

        Assert.True(Enums.TestEnum.TryParse("first", out result, TestEnumFormat.Name, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(TestEnum.First, result);

        Assert.False(Enums.TestEnum.TryParse("doesnotexist", out result));

        //Check that we also support parsing display names
        Assert.True(Enums.TestEnum.TryParse("FirstDisplayName", out result));
        Assert.Equal(TestEnum.First, result);
    }

    [Fact]
    public void ParseTest()
    {
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("First"));
        Assert.Equal(TestEnum.First, Enums.TestEnum.Parse("first", TestEnumFormat.Default, StringComparer.OrdinalIgnoreCase));
        Assert.Throws<ArgumentOutOfRangeException>(() => Enums.TestEnum.Parse("doesnotexist"));
    }

    [Fact]
    public void IsDefinedTest()
    {
        //Test flag combinations
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second | TestEnum.Third | TestEnum.Other));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second | TestEnum.Third));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First | TestEnum.Second));
        Assert.True(Enums.TestEnum.IsDefined(TestEnum.First));
        Assert.False(Enums.TestEnum.IsDefined(_invalid));

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
            nameof(TestEnum.Other)
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
            TestEnum.Other
        };

        Assert.Equal(values, Enums.TestEnum.GetMemberValues());
    }

    [Fact]
    public void GetUnderlyingValuesTest()
    {
        int[] underlyingValues =
        {
            8,
            1,
            2,
            256
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