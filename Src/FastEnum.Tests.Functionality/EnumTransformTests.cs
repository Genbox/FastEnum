using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class EnumTransformTests
{
    [Fact]
    public void TransformTest()
    {
        Assert.Equal("ThisWasOverriden", TestTransformsEnum.OverrideMe.GetString());
        Assert.Equal("UPPERCASE", TestTransformsEnum.uppercase.GetString());

        Assert.Equal(["ThisWasOverriden", "UPPERCASE"], Enums.TestTransformsEnum.GetMemberNames());
        Assert.True(Enums.TestTransformsEnum.TryParse("ThisWasOverriden", out TestTransformsEnum overridden));
        Assert.Equal(TestTransformsEnum.OverrideMe, overridden);
        Assert.True(Enums.TestTransformsEnum.TryParse("UPPERCASE", out TestTransformsEnum transformed));
        Assert.Equal(TestTransformsEnum.uppercase, transformed);
        Assert.Equal("uppercase", TestTransformsEnum.uppercase.GetString(TestTransformsEnumFormat.None));
    }
}