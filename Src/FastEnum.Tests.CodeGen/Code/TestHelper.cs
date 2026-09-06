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

    private static readonly HashSet<string> _ignore =
    [
        "CS8019"
    ];

    public static string GetGeneratedOutput<T>(string source, bool checkForErrors = true) where T : IIncrementalGenerator, new()
    {
        string res = GetGeneratedOutput<T>(source, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag);

        if (checkForErrors)
            Assert.Empty(codeGenDiag);

        if (checkForErrors)
            Assert.Empty(compilerDiag);

        return res;
    }

    public static string GetGeneratedOutput<T>(string source, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag, CSharpParseOptions? parseOptions = null) where T : IIncrementalGenerator, new()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        //Add a few headers by default
        source = GetHeader() + "\n" + source;

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions, cancellationToken: cancellationToken);
        CSharpCompilation compilation = CreateCompilation([syntaxTree]);

        T generator = new T();
        IEnumerable<ISourceGenerator> generators = [generator.AsSourceGenerator()];

        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generators, parseOptions: parseOptions);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out codeGenDiag, cancellationToken);
        compilerDiag = outputCompilation.GetDiagnostics(cancellationToken).Where(x => !_ignore.Contains(x.Id));

        List<SyntaxTree> trees = outputCompilation.SyntaxTrees.ToList();

        StringBuilder sb = new StringBuilder();

        foreach (SyntaxTree tree in trees.Skip(1))
            sb.AppendLine(tree.ToString());

        return sb.ToString();
    }

    private static string GetHeader() => _headerCache ??= File.ReadAllText(Path.Combine(_resourcesDir, "_Header.cs"));

    // Runtime references keep unit tests independent of assembly load order. Target-framework
    // compatibility is checked separately by Scripts/Test-Package.ps1 using SDK reference packs.
    internal static CSharpCompilation CreateCompilation(IEnumerable<SyntaxTree> syntaxTrees)
    {
        string platformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                                    ?? throw new InvalidOperationException("Runtime assembly references are unavailable.");
        IEnumerable<PortableExecutableReference> references = platformAssemblies.Split(Path.PathSeparator)
                                                                                .Append(typeof(EnumGenerator).Assembly.Location)
                                                                                .Append(typeof(DisplayAttribute).Assembly.Location)
                                                                                .Distinct(StringComparer.Ordinal)
                                                                                .Select(path => MetadataReference.CreateFromFile(path));

        return CSharpCompilation.Create("generator", syntaxTrees, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}