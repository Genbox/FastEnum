using Genbox.EnumSourceGen.Helpers;

namespace Genbox.EnumSourceGen.Tests.CodeGen;

public class TransformHelperTests
{
    [Theory]
    [InlineData("HelloWorld", "HelloWorld", null)]
    [InlineData("HelloWorld", "HELLOWORLD", EnumTransform.UpperCase)]
    [InlineData("HelloWorld", "helloworld", EnumTransform.LowerCase)]
    public void PresetTest(string input, string expected, EnumTransform? preset)
    {
        Assert.Equal(expected, TransformHelper.TransformName(input, preset ?? EnumTransform.None, null, null));
    }

    [Theory]
    [InlineData("HelloWorld", "HennoWorld", "/ll/nn/")]
    [InlineData("HelloWorld", "HiWorld", "/^...../Hi/")]
    public void RegexTest(string input, string expected, string? regex)
    {
        Assert.Equal(expected, TransformHelper.TransformName(input, EnumTransform.None, regex, null));
    }

    [Theory]
    [InlineData("HelloWorld", "HELLOWORLD", "UUUUUUUUUU")]
    [InlineData("HelloWorld", "helloworld", "LLLLLLLLLL")]
    [InlineData("HelloWorld", "world", "OOOOOLLLLL")]
    [InlineData("HelloWorld", "ello", "O____OOOOO")]
    [InlineData("HelloWorld", "hloWorld", "LOO")]
    public void TransformHelperTest(string input, string expected, string? casePattern)
    {
        Assert.Equal(expected, TransformHelper.TransformName(input, EnumTransform.None, null, casePattern));
    }
}