using Genbox.EnumSourceGen.Helpers;

namespace Genbox.EnumSourceGen.Tests.CodeGen;

public class HelperTests
{
    [Theory]
    [InlineData("HelloWorld", "HELLOWORLD", "HELLOWORLD", null, null)]
    [InlineData("HelloWorld", "HELLOWORLD", null, EnumTransform.UpperCase, null)]
    [InlineData("HelloWorld", "helloworld", null, EnumTransform.LowerCase, null)]
    [InlineData("HelloWorld", "HELLOWORLD", null, null, "UUUUUUUUUU")]
    [InlineData("HelloWorld", "helloworld", null, null, "LLLLLLLLLL")]
    [InlineData("HelloWorld", "world", null, null, "OOOOOLLLLL")]
    [InlineData("HelloWorld", "ello", null, null, "O____OOOOO")]
    [InlineData("HelloWorld", "HennoWorld", null, null, "/ll/nn/")]
    [InlineData("HelloWorld", "HiWorld", null, null, "/^...../Hi/")]
    public void TransformHelperTest(string input, string expected, string nameOverride, EnumTransform? simpleTransform, string? advancedTransform)
    {
        string actual = TransformHelper.TransformName(input, nameOverride, simpleTransform ?? EnumTransform.None, advancedTransform);
        Assert.Equal(expected, actual);
    }
}