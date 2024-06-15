using Genbox.FastEnum.Tests.Functionality.Code;

namespace Genbox.FastEnum.Tests.Functionality;

public class EnumTransformTests
{
    [Fact]
    public void TransformTest()
    {
        Assert.Equal("ThisWasOverriden", TestTransformsEnum.OverrideMe.GetString());
        Assert.Equal("UPPERCASE", TestTransformsEnum.uppercase.GetString());
    }
}