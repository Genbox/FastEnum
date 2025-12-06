using System.Globalization;
using Genbox.FastEnum.Tests.CodeGen.Code;
using Xunit;

namespace Genbox.FastEnum.Tests.CodeGen;

public class InvariantCultureTests
{
    [Fact]
    public void ValueParseUsesInvariantCulture()
    {
        CultureInfo original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("ar-EG");

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

            Assert.Contains("value.Equals(\"-1234\"", output);
            Assert.Contains("value.Equals(\"1234567890\"", output);
            Assert.DoesNotContain('١', output); //Arabic-Indic digit one; indicates culture bleed
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
