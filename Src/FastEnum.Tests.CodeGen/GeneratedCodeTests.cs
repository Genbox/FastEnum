using Genbox.FastEnum.Tests.CodeGen.Code;

namespace Genbox.FastEnum.Tests.CodeGen;

public class GeneratedCodeTests
{
    [Fact]
    public void AttributeContainsNameAndVersion()
    {
        const string code = """
                            [FastEnum]
                            public enum TestEnum
                            {
                                Value
                            }
                            """;

        string output = TestHelper.GetGeneratedOutput<EnumGenerator>(code);
        string version = typeof(EnumGenerator).Assembly.GetName().Version!.ToString();
        string attribute = $"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"FastEnum\", \"{version}\")]";

        Assert.Equal(4, output.Split(attribute).Length - 1);
    }
}