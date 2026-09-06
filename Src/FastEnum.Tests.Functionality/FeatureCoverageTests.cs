using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class FeatureCoverageTests
{
    [Fact]
    public void CollectionsAreCached()
    {
        Assert.Same(Enums.TestEnum.GetMemberNames(), Enums.TestEnum.GetMemberNames());
        Assert.Same(Enums.TestEnum.GetMemberValues(), Enums.TestEnum.GetMemberValues());
        Assert.Same(Enums.TestEnum.GetUnderlyingValues(), Enums.TestEnum.GetUnderlyingValues());
        Assert.Same(Enums.TestEnum.GetDisplayNames(), Enums.TestEnum.GetDisplayNames());
        Assert.Same(Enums.TestEnum.GetDescriptions(), Enums.TestEnum.GetDescriptions());
    }

    [Fact]
    public void CollectionsHonorConfiguredOrdering()
    {
        Assert.Equal(["One", "Three", "Two"], Enums.SortedEnum.GetMemberNames());
        Assert.Equal([SortedEnum.Three, SortedEnum.One, SortedEnum.Two], Enums.SortedEnum.GetMemberValues());
        Assert.Equal([1, 2, 3], Enums.SortedEnum.GetUnderlyingValues());
        Assert.Equal([(SortedEnum.Three, "Charlie"), (SortedEnum.One, "Bravo"), (SortedEnum.Two, "Alpha")], Enums.SortedEnum.GetDisplayNames());
        Assert.Equal([(SortedEnum.Two, "First"), (SortedEnum.One, "Second"), (SortedEnum.Three, "Third")], Enums.SortedEnum.GetDescriptions());
    }

    [Fact]
    public void EveryOmissionTargetIsHonored()
    {
        Assert.Equal(11, Enums.OmitCoverageEnum.MemberCount);

        Assert.DoesNotContain(nameof(OmitCoverageEnum.OmitAll), Enums.OmitCoverageEnum.GetMemberNames(), StringComparer.Ordinal);
        Assert.DoesNotContain(nameof(OmitCoverageEnum.NoMemberName), Enums.OmitCoverageEnum.GetMemberNames(), StringComparer.Ordinal);
        Assert.DoesNotContain(OmitCoverageEnum.OmitAll, Enums.OmitCoverageEnum.GetMemberValues());
        Assert.DoesNotContain(OmitCoverageEnum.NoMemberValue, Enums.OmitCoverageEnum.GetMemberValues());
        Assert.DoesNotContain(1, Enums.OmitCoverageEnum.GetUnderlyingValues());
        Assert.DoesNotContain(4, Enums.OmitCoverageEnum.GetUnderlyingValues());

        Assert.False(OmitCoverageEnum.OmitAll.TryGetUnderlyingValue(out _));
        Assert.False(OmitCoverageEnum.NoUnderlyingLookup.TryGetUnderlyingValue(out _));
        Assert.False(Enums.OmitCoverageEnum.TryParse(nameof(OmitCoverageEnum.OmitAll), out _));
        Assert.False(Enums.OmitCoverageEnum.TryParse(nameof(OmitCoverageEnum.NoParse), out _));

        Assert.True(OmitCoverageEnum.Keep.TryGetDisplayName(out string? displayName));
        Assert.Equal("Keep display", displayName);
        Assert.False(OmitCoverageEnum.NoDisplayLookup.TryGetDisplayName(out _));
        Assert.True(OmitCoverageEnum.Keep.TryGetDescription(out string? description));
        Assert.Equal("Keep description", description);
        Assert.False(OmitCoverageEnum.NoDescriptionLookup.TryGetDescription(out _));

        Assert.False(Enums.OmitCoverageEnum.IsDefined(OmitCoverageEnum.OmitAll));
        Assert.False(Enums.OmitCoverageEnum.IsDefined(OmitCoverageEnum.NoDefined));
        Assert.Equal(string.Empty, OmitCoverageEnum.OmitAll.GetString());
        Assert.Equal(string.Empty, OmitCoverageEnum.NoString.GetString());

        Assert.Contains(nameof(OmitCoverageEnum.NoOmission), Enums.OmitCoverageEnum.GetMemberNames(), StringComparer.Ordinal);
        Assert.Contains(OmitCoverageEnum.NoOmission, Enums.OmitCoverageEnum.GetMemberValues());
        Assert.True(OmitCoverageEnum.NoOmission.TryGetUnderlyingValue(out int underlyingValue));
        Assert.Equal(11, underlyingValue);
        Assert.True(Enums.OmitCoverageEnum.TryParse(nameof(OmitCoverageEnum.NoOmission), out _));
        Assert.True(Enums.OmitCoverageEnum.IsDefined(OmitCoverageEnum.NoOmission));
        Assert.Equal(nameof(OmitCoverageEnum.NoOmission), OmitCoverageEnum.NoOmission.GetString());
    }
}