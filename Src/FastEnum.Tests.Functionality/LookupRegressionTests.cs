using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class LookupRegressionTests
{
    [Fact]
    public void UnderlyingLookupUsesIncludedNumericAliases()
    {
        Assert.True(UnderlyingAliasEnum.First.TryGetUnderlyingValue(out int value));
        Assert.Equal(1, value);
        Assert.Equal(1, UnderlyingAliasEnum.Second.GetUnderlyingValue());
        Assert.False(UnderlyingAliasEnum.Excluded.TryGetUnderlyingValue(out _));
        Assert.False(((UnderlyingAliasEnum)3).TryGetUnderlyingValue(out _));
    }

    [Theory]
    [InlineData(0L, false)]
    [InlineData(1L, true)]
    [InlineData(2L, true)]
    [InlineData(3L, false)]
    [InlineData(4L, true)]
    [InlineData(5L, true)]
    [InlineData(8L, false)]
    [InlineData(long.MinValue, true)]
    [InlineData(long.MinValue + 1, false)]
    [InlineData(long.MinValue + 2, true)]
    public void FlagValidationHonorsOmissionsAndIncludedAliases(long rawValue, bool expected)
    {
        Assert.Equal(expected, Enums.OmittedValidationFlags.IsDefined((OmittedValidationFlags)rawValue));
    }

    [Fact]
    public void FlagUnderlyingLookupRejectsOmittedAliasesAndAllowsCombinations()
    {
        Assert.False(OmittedValidationFlags.Both.TryGetUnderlyingValue(out _));
        Assert.False(OmittedValidationFlags.BothAlias.TryGetUnderlyingValue(out _));
        Assert.Equal(4, OmittedValidationFlags.Third.GetUnderlyingValue());
        Assert.Equal(5, (OmittedValidationFlags.First | OmittedValidationFlags.Third).GetUnderlyingValue());
    }

    [Fact]
    public void FullyOmittedFlagsHaveNoDefinedValues()
    {
        Assert.False(Enums.FullyOmittedFlags.IsDefined(FullyOmittedFlags.None));
        Assert.False(Enums.FullyOmittedFlags.IsDefined(FullyOmittedFlags.First));
    }
}