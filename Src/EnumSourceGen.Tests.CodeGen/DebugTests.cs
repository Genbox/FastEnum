using Genbox.EnumSourceGen.Tests.CodeGen.Code;

namespace Genbox.EnumSourceGen.Tests.CodeGen;

public class DebugTests
{
    private readonly ITestOutputHelper _output;

    public DebugTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CodeGenDemo()
    {
        string source = """
namespace Some.Namespace.Here2
{
    [EnumSourceGen]
    public enum MyEnum
    {
        Value,
        Value2
    }
}
""";

        string actual = TestHelper.GetGeneratedOutput<EnumGenerator>(source);
        _output.WriteLine(actual);
    }
}