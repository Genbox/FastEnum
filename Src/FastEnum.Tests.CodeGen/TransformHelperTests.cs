using Genbox.FastEnum.Helpers;

namespace Genbox.FastEnum.Tests.CodeGen;

public class TransformHelperTests
{
    [Theory]
    [InlineData("HelloWorld", EnumTransform.None)]
    [InlineData("HELLOWORLD", EnumTransform.UpperCase)]
    [InlineData("helloworld", EnumTransform.LowerCase)]
    public void PresetTest(string expected, EnumTransform preset) => Assert.Equal(expected, TransformHelper.TransformName("HelloWorld", preset, null, null));

    [Theory]
    [InlineData("HennoWorld", "/ll/nn/")]
    [InlineData("HiWorld", "/^...../Hi/")]
    public void RegexTest(string expected, string? regex) => Assert.Equal(expected, TransformHelper.TransformName("HelloWorld", EnumTransform.None, regex, null));

    [Theory]
    [InlineData("HELLOWORLD", "UUUUUUUUUU")]
    [InlineData("helloworld", "LLLLLLLLLL")]
    [InlineData("world", "OOOOOLLLLL")]
    [InlineData("ello", "O____OOOOO")]
    [InlineData("hloWorld", "LOO")]
    public void TransformHelperTest(string expected, string? casePattern) => Assert.Equal(expected, TransformHelper.TransformName("HelloWorld", EnumTransform.None, null, casePattern));
}