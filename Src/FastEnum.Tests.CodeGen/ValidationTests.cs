using System.Collections.Immutable;
using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;

namespace Genbox.FastEnum.Tests.CodeGen;

public class ValidationTests
{
    /// <summary>This test ensure that a public enum in an internal class results in an error.</summary>
    [Fact]
    public void TestLessVisibleClass()
    {
        string code = """
                      internal class MyInternalClass
                      {
                          [FastEnum]
                          public enum MyPublicEnum1
                          {
                              Value
                          }
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id); //Enum visibility validation failed
    }

    /// <summary>This test ensure that am internal enum where the user overrides either the enums class to be more visible</summary>
    [Fact]
    public void TestVisibleEnumsClassOverride()
    {
        string code = """
                      [FastEnum(EnumsClassVisibility = Visibility.Public)]
                      internal enum MyPublicEnum
                      {
                          Value
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id); //Enum visibility validation failed
    }

    /// <summary>This test ensure that am internal enum where the user overrides either the extensions class to be more visible</summary>
    [Fact]
    public void TestVisibleExtensionsClassOverride()
    {
        string code = """
                      [FastEnum(ExtensionClassVisibility = Visibility.Public)]
                      internal enum MyPublicEnum
                      {
                          Value
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id); //Enum visibility validation failed
    }

    /// <summary>Ensures shared partial wrapper declarations use compatible visibility.</summary>
    [Fact]
    public void TestSharedEnumsClassVisibility()
    {
        string code = """
                      [FastEnum]
                      internal enum InternalEnum
                      {
                          Value
                      }

                      [FastEnum]
                      public enum PublicEnum
                      {
                          Value
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Assert.Empty(codeGenDiag);
        Assert.Empty(compilerDiag);
    }

    /// <summary>Ensures a format enum is visible wherever a generated public API exposes it.</summary>
    [Fact]
    public void TestFormatEnumVisibility()
    {
        string code = """
                      [FastEnum(EnumsClassVisibility = Visibility.Internal, ExtensionClassVisibility = Visibility.Public)]
                      public enum PublicEnum
                      {
                          Value
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Assert.Empty(codeGenDiag);
        Assert.Empty(compilerDiag);
    }
}