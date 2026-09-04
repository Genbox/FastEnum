using System.Collections.Immutable;
using Genbox.FastEnum.Tests.CodeGen.Code;
using Microsoft.CodeAnalysis;

namespace Genbox.FastEnum.Tests.CodeGen;

public class ValidationTests
{
    [Theory]
    [InlineData("EnumsClassVisibility")]
    [InlineData("ExtensionClassVisibility")]
    public void TestPublicOverrideCannotExposeInternalAncestor(string propertyName)
    {
        string code = $$"""
                        internal class Outer
                        {
                            public class Inner
                            {
                                [FastEnum({{propertyName}} = Visibility.Public)]
                                public enum Nested { None }
                            }
                        }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> diagnostics, out IEnumerable<Diagnostic> compilerDiagnostics);
        Assert.Empty(compilerDiagnostics);
        Assert.Equal("FE001", Assert.Single(diagnostics).Id);
    }

    [Theory]
    [InlineData("internal", "public", "public", true)]
    [InlineData("public", "private", "internal", false)]
    [InlineData("public", "protected", "internal", false)]
    [InlineData("public", "protected internal", "internal", true)]
    [InlineData("public", "protected internal", "public", true)]
    [InlineData("public", "public", "protected internal", true)]
    [InlineData("public", "public", "public", true)]
    [InlineData("internal", "public", "internal", true)]
    public void TestAncestorVisibility(string outerAccess, string middleAccess, string enumAccess, bool supported)
    {
        string code = $$"""
                        {{outerAccess}} class Outer
                        {
                            {{middleAccess}} class Middle
                            {
                                public class Inner
                                {
                                    [FastEnum]
                                    {{enumAccess}} enum Nested { None }
                                }
                            }
                        }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> diagnostics, out IEnumerable<Diagnostic> compilerDiagnostics);
        Assert.Empty(compilerDiagnostics);
        if (supported)
            Assert.Empty(diagnostics);
        else
            Assert.Equal("FE001", Assert.Single(diagnostics).Id);
    }

    /// <summary>A public enum in an internal class gets internal helpers.</summary>
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

        string output = TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Assert.Empty(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Contains("internal static partial class MyPublicEnum1Extensions", output, StringComparison.Ordinal);
        Assert.Contains("internal enum MyPublicEnum1Format", output, StringComparison.Ordinal);
    }

    /// <summary>Public helper overrides cannot expose an internal enum.</summary>
    [Theory]
    [InlineData("EnumsClassVisibility")]
    [InlineData("ExtensionClassVisibility")]
    public void TestPublicOverrideCannotExposeInternalEnum(string propertyName)
    {
        string code = $$"""
                        [FastEnum({{propertyName}} = Visibility.Public)]
                        internal enum MyEnum { Value }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> diagnostics, out IEnumerable<Diagnostic> compilerDiagnostics);
        Assert.Empty(compilerDiagnostics);
        Assert.Equal("FE001", Assert.Single(diagnostics).Id);
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

    /// <summary>Ensures multiple enums can contribute overloads to one partial extension class.</summary>
    [Fact]
    public void TestSharedExtensionClass()
    {
        string code = """
                      namespace Example;

                      [FastEnum(ExtensionClassName = "SharedExtensions")]
                      public enum First
                      {
                          Value
                      }

                      [FastEnum(ExtensionClassName = "SharedExtensions")]
                      public enum Second
                      {
                          Value
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Assert.Empty(codeGenDiag);
        Assert.Empty(compilerDiag);
    }

    /// <summary>Ensures shared partial extension declarations use compatible visibility.</summary>
    [Fact]
    public void TestSharedExtensionClassVisibility()
    {
        string code = """
                      [FastEnum(ExtensionClassName = "SharedExtensions")]
                      public enum PublicEnum
                      {
                          Value
                      }

                      [FastEnum(ExtensionClassName = "SharedExtensions")]
                      internal enum InternalEnum
                      {
                          Value
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id);
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

    /// <summary>Ensures enums inside generic containing types produce a validation diagnostic.</summary>
    [Fact]
    public void TestGenericContainingType()
    {
        string code = """
                      public class GenericContainer<T>
                      {
                          [FastEnum]
                          public enum MyEnum
                          {
                              Value
                          }
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id);
    }

    /// <summary>Ensures unsupported nested enum accessibilities produce a validation diagnostic.</summary>
    [Theory]
    [InlineData("private")]
    [InlineData("protected")]
    [InlineData("private protected")]
    public void TestUnsupportedEnumAccessibility(string accessibility)
    {
        string code = $$"""
                        public class Container
                        {
                            [FastEnum]
                            {{accessibility}} enum MyEnum
                            {
                                Value
                            }
                        }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id);
    }

    /// <summary>Ensures generated type-name collisions produce a validation diagnostic.</summary>
    [Theory]
    [InlineData("Alpha")]
    [InlineData("@Alpha")]
    public void TestGeneratedNameCollision(string generatedName)
    {
        string code = $$"""
                        namespace First
                        {
                            [FastEnum(EnumsClassNamespace = "Shared")]
                            public enum Alpha
                            {
                                Value
                            }
                        }

                        namespace Second
                        {
                            [FastEnum(EnumsClassNamespace = "Shared", EnumNameOverride = "{{generatedName}}")]
                            public enum Beta
                            {
                                Value
                            }
                        }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id);
    }

    /// <summary>Ensures escaped enum and member identifiers are emitted correctly.</summary>
    [Fact]
    public void TestEscapedIdentifiers()
    {
        string code = """
                      [FastEnum(EnumNameOverride = "@class")]
                      public enum @event
                      {
                          @class
                      }
                      """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Assert.Empty(codeGenDiag);
        Assert.Empty(compilerDiag);
    }

    [Theory]
    [InlineData("EnumNameOverride", "Bad-Name")]
    [InlineData("EnumsClassName", "Bad Name")]
    [InlineData("ExtensionClassName", "1BadName")]
    [InlineData("EnumsClassNamespace", "Good..Bad")]
    [InlineData("ExtensionClassNamespace", "Good.Bad-Name")]
    public void TestInvalidOverrides(string propertyName, string value)
    {
        string code = $$"""
                        [FastEnum({{propertyName}} = "{{value}}")]
                        public enum MyEnum
                        {
                            Value
                        }
                        """;

        TestHelper.GetGeneratedOutput<EnumGenerator>(code, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);
        Diagnostic res = Assert.Single(codeGenDiag);
        Assert.Empty(compilerDiag);
        Assert.Equal("FE001", res.Id);
    }
}