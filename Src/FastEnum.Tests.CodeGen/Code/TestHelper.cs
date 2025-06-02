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

    public static string GetGeneratedOutput<T>(string source, out ImmutableArray<Diagnostic> codeGenDiag, out IEnumerable<Diagnostic> compilerDiag) where T : IIncrementalGenerator, new()
    {
        //Add a few headers by default
        source = GetHeader() + "\n" + source;

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        IEnumerable<PortableExecutableReference> refs = AppDomain.CurrentDomain.GetAssemblies()
                                                                 .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                                                                 .Select(x => MetadataReference.CreateFromFile(x.Location))
                                                                 .Concat(new[]
                                                                 {
                                                                     MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
                                                                     MetadataReference.CreateFromFile(typeof(DisplayAttribute).Assembly.Location),
                                                                     MetadataReference.CreateFromFile(typeof(FlagsAttribute).Assembly.Location)
                                                                 });

        CSharpCompilation compilation = CSharpCompilation.Create(
            "generator",
            new[] { syntaxTree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        T generator = new T();

        CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out codeGenDiag);
        compilerDiag = outputCompilation.GetDiagnostics().Where(x => !_ignore.Contains(x.Id));

        List<SyntaxTree> trees = outputCompilation.SyntaxTrees.ToList();

        StringBuilder sb = new StringBuilder();

        foreach (SyntaxTree tree in trees.Skip(1))
            sb.AppendLine(tree.ToString());

        return sb.ToString();
    }

    private static string GetHeader()
    {
        return _headerCache ??= File.ReadAllText(Path.Combine(_resourcesDir, "_Header.dat"));
    }
}