using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Genbox.FastEnum.Tests.CodeGen.Code;

internal static class TestHelper
{
    private static string? _headerCache;
    private static readonly string _resourcesDir = AppContext.BaseDirectory + "../../../Resources";

    private static readonly ImmutableArray<PortableExecutableReference> _references = CreateReferences();

    public static string GetGeneratedOutput(string source, CSharpParseOptions? parseOptions = null)
    {
        string res = GetGeneratedOutput(source, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag, parseOptions);

        Assert.Empty(codeGenDiag);
        Assert.Empty(compilerDiag);

        return res;
    }

    public static string GetGeneratedOutput(string source, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag, CSharpParseOptions? parseOptions = null)
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        //Add a few headers by default
        source = GetHeader() + "\n" + source;

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions, cancellationToken: cancellationToken);
        CSharpCompilation compilation = CreateCompilation([syntaxTree]);

        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create([new EnumGenerator().AsSourceGenerator()], parseOptions: parseOptions);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out codeGenDiag, cancellationToken);
        compilerDiag = outputCompilation.GetDiagnostics(cancellationToken).Where(x => x.Id != "CS8019");

        StringBuilder sb = new StringBuilder();

        foreach (SyntaxTree tree in outputCompilation.SyntaxTrees.Skip(1))
            sb.AppendLine(tree.ToString());

        return sb.ToString();
    }

    private static string GetHeader() => _headerCache ??= File.ReadAllText(Path.Combine(_resourcesDir, "_Header.cs"));

    internal static CSharpCompilation CreateCompilation(IEnumerable<SyntaxTree> syntaxTrees) => CSharpCompilation.Create("generator", syntaxTrees, _references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    // Runtime references keep unit tests independent of assembly load order. Target-framework
    // compatibility is checked separately by Scripts/Test-Package.ps1 using SDK reference packs.
    private static ImmutableArray<PortableExecutableReference> CreateReferences()
    {
        string platformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                                    ?? throw new InvalidOperationException("Runtime assembly references are unavailable.");
        return platformAssemblies.Split(Path.PathSeparator)
                                 .Append(typeof(EnumGenerator).Assembly.Location)
                                 .Append(typeof(DisplayAttribute).Assembly.Location)
                                 .Distinct(StringComparer.Ordinal)
                                 .Select(path => MetadataReference.CreateFromFile(path))
                                 .ToImmutableArray();
    }
}