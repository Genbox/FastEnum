using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Genbox.EnumSourceGen.Tests.Code;

internal static class TestHelper
{
    private static string? _headerCache;

    private static string GetHeader()
    {
        return _headerCache ??= ReadResource("Genbox.EnumSourceGen.Tests.Resources._Header.dat");
    }

    public static void TestResource<T>(string resourceName) where T : IIncrementalGenerator, new()
    {
        string inputSource = ReadResource(resourceName);

        //Add the header
        inputSource = GetHeader() + "\n" + inputSource;

        string actual = GetGeneratedOutput<T>(inputSource).ReplaceLineEndings("\n");
        string expected = ReadResource(Path.ChangeExtension(resourceName, "output"));

        Assert.Equal(expected, actual);
    }

    private static string ReadResource(string name)
    {
        Assembly assembly = typeof(TestHelper).Assembly;

        using Stream? stream = assembly.GetManifestResourceStream(name);

        if (stream == null)
            throw new InvalidOperationException("Unable to find the resource " + name);

        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static string GetGeneratedOutput<T>(string source) where T : IIncrementalGenerator, new()
    {
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
        driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);
        Assert.Empty(diagnostics);

        List<SyntaxTree> trees = outputCompilation.SyntaxTrees.ToList();
        Assert.True(trees.Count > 1);

        StringBuilder sb = new StringBuilder();

        foreach (SyntaxTree tree in trees.Skip(1))
            sb.AppendLine(tree.ToString());

        return sb.ToString();
    }
}