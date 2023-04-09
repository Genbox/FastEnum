using Genbox.EnumSourceGen.Tests.Functionality.Code;

namespace Genbox.EnumSourceGen.Tests.Functionality;

public class EnumTransformTests
{
    [Fact]
    public void TransformTest()
    {
        Assert.Equal("ThisWasOverriden", TestTransformsEnum.OverrideMe.GetString());
        Assert.Equal("UPPERCASE", TestTransformsEnum.UpperCase.GetString());
        Assert.Equal("lowercase", TestTransformsEnum.LowerCase.GetString());
        Assert.Equal("MiXeDCase", TestTransformsEnum.MixedCase1.GetString());
        Assert.Equal("RemoveAllOnes", TestTransformsEnum.R1e1m1o1v1e1A1l1l1O1n1e1s.GetString());
        Assert.Equal("", TestTransformsEnum.Omitted.GetString());
        Assert.Equal("OmittedWithFilter", TestTransformsEnum.OmittedWithFilter.GetString());
    }
}