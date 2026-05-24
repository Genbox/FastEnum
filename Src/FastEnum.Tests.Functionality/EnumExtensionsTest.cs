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

        // Omitted value should stay omitted regardless of format selection
        Assert.Equal(string.Empty, TestOmitEnum.Omitted.GetString(TestOmitEnumFormat.Name));
        Assert.Equal(string.Empty, TestOmitEnum.Omitted.GetString(TestOmitEnumFormat.Value));
    }
}