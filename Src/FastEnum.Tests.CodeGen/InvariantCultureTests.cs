using System.Globalization;
using Genbox.FastEnum.Tests.CodeGen.Code;

namespace Genbox.FastEnum.Tests.CodeGen;

public class InvariantCultureTests
{
    [Fact]
    public void ValueParseUsesInvariantCulture()
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-EG");

        try
        {
            const string code = """
                                [FastEnum]
                                public enum TestEnum : long
                                {
                                    Negative = -1234,
                                    Positive = 1234567890
                                }
                                """;

            string output = TestHelper.GetGeneratedOutput<EnumGenerator>(code);

            Assert.Contains(".Equals(\"-1234\"", output, StringComparison.Ordinal);
            Assert.Contains(".Equals(\"1234567890\"", output, StringComparison.Ordinal);
            Assert.DoesNotContain('١', output); //Arabic-Indic digit one; indicates culture bleed
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}