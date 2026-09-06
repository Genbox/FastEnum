using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class HashLookupTests
{
    [Fact]
    public void SignedLookupPreservesAliasesOmissionsAndLimits()
    {
        foreach (LargeLookupEnum value in Enum.GetValues<LargeLookupEnum>())
            Assert.Equal(value != LargeLookupEnum.Value5, Enums.LargeLookupEnum.IsDefined(value));

        Assert.False(Enums.LargeLookupEnum.IsDefined((LargeLookupEnum)(long.MinValue + 1)));
        Assert.False(Enums.LargeLookupEnum.IsDefined((LargeLookupEnum)(long.MaxValue - 1)));
        Assert.False(Enums.LargeLookupEnum.IsDefined((LargeLookupEnum)1));
    }

    [Fact]
    public void UnsignedLookupUsesHighBitsAndChecksCollisionChains()
    {
        foreach (HighBitHashEnum value in Enum.GetValues<HighBitHashEnum>())
            Assert.True(Enums.HighBitHashEnum.IsDefined(value));

        // These misses share buckets with declared values; a bucket hit alone is insufficient.
        Assert.False(Enums.HighBitHashEnum.IsDefined((HighBitHashEnum)1UL));
        Assert.False(Enums.HighBitHashEnum.IsDefined((HighBitHashEnum)((1UL << 63) + 1)));
        Assert.False(Enums.HighBitHashEnum.IsDefined((HighBitHashEnum)(ulong.MaxValue - 1)));
    }

    [Fact]
    public void PublicMetadataArraysCannotCorruptHashLookup()
    {
        ulong[] values = Enums.HighBitHashEnum.GetUnderlyingValues();
        ulong original = values[0];

        try
        {
            values[0] = 1;
            Assert.True(Enums.HighBitHashEnum.IsDefined(HighBitHashEnum.Value0));
            Assert.False(Enums.HighBitHashEnum.IsDefined((HighBitHashEnum)1));
        }
        finally
        {
            values[0] = original;
        }
    }
}