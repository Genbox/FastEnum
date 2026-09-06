using Genbox.FastEnum.Data;
using Microsoft.CodeAnalysis;

namespace Genbox.FastEnum.Tests.CodeGen;

public class EnumSpecTests
{
    [Fact]
    public void EqualityHandlesNullAndUnrelatedTypes()
    {
        EnumSpec spec = Create();
        Assert.False(spec.Equals(null));
        Assert.False(spec.Equals((object?)null));
        Assert.False(spec.Equals(new object()));
        Assert.True(spec.Equals(spec));
    }

    [Fact]
    public void IndependentlyAllocatedSpecsHaveStructuralEquality()
    {
        EnumSpec first = Create();
        EnumSpec second = Create();
        Assert.True(first.Equals(second));
        Assert.True(second.Equals(first));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.False(first.Equals(Create(1)));
    }

    private static EnumSpec Create(int value = 0) => new EnumSpec("Color", "Color", "Color", "global::Color", null,
        [Accessibility.Public], false, false, false, false, "int", new FastEnumData(),
        [new EnumMemberSpec("Red", "Red", value, null, null, null)], null);
}