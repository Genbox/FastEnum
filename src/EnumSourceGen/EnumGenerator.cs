using System.Collections.Immutable;
using System.Text;
using Genbox.EnumSourceGen.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Genbox.EnumSourceGen;

internal readonly record struct EnumSpec(string EnumName, string EnumFullName, string EnumFullyQualifiedName, string EnumNamespace, Generate Generate, string ExtName, string? ExtNamespace, string EnumsClassName, string EnumsClassNamespace, bool IsPublic, bool HasDisplay, bool HasDescription, bool HasFlags, string UnderlyingType, List<EnumMember> Members);
internal readonly record struct EnumMember(string Name, object? Value, string? DisplayName, string? Description);

[Generator]
public class EnumGenerator : IIncrementalGenerator
{
    private const string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";
    private const string FlagsAttribute = "System.FlagsAttribute";
    private const string EnumExtensionsAttribute = "Genbox.EnumSourceGen.EnumSourceGenAttribute";

    public const bool Debug = false;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<CompilationInfo> cp = context.CompilationProvider
                                                              .Select(static (c, _) => new CompilationInfo(c.GetTypeByMetadataName(EnumExtensionsAttribute), c.GetTypeByMetadataName(DisplayAttribute), c.GetTypeByMetadataName(FlagsAttribute)));

        IncrementalValuesProvider<ISymbol> sp = context.SyntaxProvider
                                                       .CreateSyntaxProvider(Predicate, Transform)
                                                       .Where(s => s != null)!;

        Dictionary<string, int> uniq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        StringBuilder sb = new StringBuilder();

        context.RegisterSourceOutput(sp.Combine(cp), (spc, source) =>
        {
            if (!TryGetTypesToGenerate(source.Right, source.Left, out EnumSpec enumSpec))
                return;

            sb.Clear();

            string safeName = enumSpec.EnumFullName;

            if (uniq.TryGetValue(enumSpec.EnumFullName, out int count))
                uniq[enumSpec.EnumFullName] = ++count;
            else
                uniq.Add(enumSpec.EnumFullName, 0);

            if (count > 0)
                safeName += count;

            switch (enumSpec.Generate)
            {
                case Generate.ClassAndExtensions:
                    spc.AddSource(safeName + $"_Enums{(Debug ? null : ".g")}.cs", SourceText.From(EnumClassCode.Generate(enumSpec, sb), Encoding.UTF8));
                    spc.AddSource(safeName + $"_Extensions{(Debug ? null : ".g")}.cs", SourceText.From(ExtensionCode.Generate(enumSpec, sb), Encoding.UTF8));
                    break;
                case Generate.ClassOnly:
                    spc.AddSource(safeName + $"_Enums{(Debug ? null : ".g")}.cs", SourceText.From(EnumClassCode.Generate(enumSpec, sb), Encoding.UTF8));
                    break;
                case Generate.ExtensionsOnly:
                    spc.AddSource(safeName + $"_Extensions{(Debug ? null : ".g")}.cs", SourceText.From(ExtensionCode.Generate(enumSpec, sb), Encoding.UTF8));
                    break;
                default:
                    throw new InvalidOperationException($"Value '{enumSpec.Generate}' is outside of supported values");
            }
        });
    }

    private static bool Predicate(SyntaxNode node, CancellationToken _) => node is EnumDeclarationSyntax m && m.AttributeLists.Count > 0;

    private static ISymbol? Transform(GeneratorSyntaxContext context, CancellationToken token)
    {
        EnumDeclarationSyntax syntax = (EnumDeclarationSyntax)context.Node;

        for (int i = 0; i < syntax.AttributeLists.Count; i++)
        {
            AttributeListSyntax list = syntax.AttributeLists[i];

            for (int j = 0; j < list.Attributes.Count; j++)
            {
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(list.Attributes[j], token);

                if (symbolInfo.Symbol == null)
                    throw new InvalidCastException("BUG");

                INamedTypeSymbol ns = symbolInfo.Symbol.ContainingType;
                string fullName = ns.ToDisplayString();

                if (string.Equals(fullName, EnumExtensionsAttribute, StringComparison.Ordinal))
                    return context.SemanticModel.GetDeclaredSymbol(syntax, token);
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    private static bool TryGetTypesToGenerate(CompilationInfo info, ISymbol? symbol, out EnumSpec enumSpec)
    {
        // nothing to do if this type isn't available
        if (info.EnumAttr == null)
        {
            enumSpec = default;
            return false;
        }

        if (symbol is not INamedTypeSymbol enumSymbol)
            throw new InvalidOperationException("BUG");

        string setExtName = enumSymbol.Name + "Extensions";
        string setEnumsName = "Enums";

        string? setExtNamespace = enumSymbol.ContainingNamespace.IsGlobalNamespace ? null : enumSymbol.ContainingNamespace.ToDisplayString();
        string setEnumsNamespace = "Genbox.EnumSourceGen";

        Generate setGenerate = Generate.ClassAndExtensions;

        //We now read attributes applied directly to the enum itself
        INamedTypeSymbol? flagsAttr = info.FlagsAttr;
        bool hasFlags = false;

        ImmutableArray<AttributeData> sAttrs = enumSymbol.GetAttributes();

        for (int i = 0; i < sAttrs.Length; i++)
        {
            AttributeData ad = sAttrs[i];

            if (flagsAttr != null && flagsAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
            {
                hasFlags = true;
                continue;
            }

            if (!enumSymbol.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                continue;

            for (int j = 0; j < ad.NamedArguments.Length; j++)
            {
                KeyValuePair<string, TypedConstant> na = ad.NamedArguments[j];

                if (na.Value.Value == null)
                    continue;

                switch (na.Key)
                {
                    case nameof(EnumSourceGenAttribute.Generate):
                        setGenerate = (Generate)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.ExtensionClassName):
                        setExtName = (string)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.ExtensionClassNamespace):
                        setExtNamespace = (string?)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.EnumsClassName):
                        setEnumsName = (string)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.EnumsClassNamespace):
                        setEnumsNamespace = (string)na.Value.Value;
                        break;
                    default:
                        throw new ArgumentException("BUG: Unsupported value");
                }
            }
        }

        //Now we read attributes applied to members of the enum
        ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
        List<EnumMember> members = new List<EnumMember>(enumMembers.Length);

        INamedTypeSymbol? displayAttr = info.DisplayAttr;

        bool hasDisplay = false;
        bool hasDescription = false;

        for (int i = 0; i < enumMembers.Length; i++)
        {
            ISymbol member = enumMembers[i];

            if (member is not IFieldSymbol field || field.ConstantValue == null)
                continue;

            string? displayName = null;
            string? description = null;

            if (displayAttr != null)
            {
                ImmutableArray<AttributeData> mAttrs = member.GetAttributes();

                for (int j = 0; j < mAttrs.Length; j++)
                {
                    AttributeData ad = mAttrs[j];

                    if (!displayAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                        continue;

                    for (int k = 0; k < ad.NamedArguments.Length; k++)
                    {
                        KeyValuePair<string, TypedConstant> na = ad.NamedArguments[k];

                        if (na.Key == "Name" && na.Value.Value is string val1)
                            displayName = val1;
                        else if (na.Key == "Description" && na.Value.Value is string val2)
                            description = val2;
                    }
                }
            }

            hasDisplay |= displayName != null;
            hasDescription |= description != null;

            members.Add(new EnumMember(member.Name, field.ConstantValue, displayName, description));
        }

        string underlyingType = enumSymbol.EnumUnderlyingType?.Name ?? "int";
        bool isPublic = enumSymbol.DeclaredAccessibility == Accessibility.Public;

        ImmutableArray<SymbolDisplayPart> parts = enumSymbol.ToDisplayParts();

        StringBuilder fqnSb = new StringBuilder(50);
        StringBuilder namespaceSb = new StringBuilder(25);
        StringBuilder enumFullSb = new StringBuilder(25);

        bool inNamespace = false;
        for (int i = 0; i < parts.Length; i++)
        {
            SymbolDisplayPart part = parts[i];

            switch (part.Kind)
            {
                case SymbolDisplayPartKind.NamespaceName:
                    inNamespace = true;
                    break;
                case SymbolDisplayPartKind.ClassName:
                case SymbolDisplayPartKind.EnumName:
                    inNamespace = false;
                    break;
                case SymbolDisplayPartKind.Punctuation:
                    break;
            }

            if (inNamespace)
                namespaceSb.Append(part);
            else
                enumFullSb.Append(part);

            fqnSb.Append(part);
        }

        string fqn = fqnSb.ToString();
        string enumNamespace = namespaceSb.ToString().TrimEnd('.');
        string enumFullName = enumFullSb.ToString();

        enumSpec = new EnumSpec(enumSymbol.Name, enumFullName, fqn, enumNamespace, setGenerate, setExtName, setExtNamespace, setEnumsName, setEnumsNamespace, isPublic, hasDisplay, hasDescription, hasFlags, underlyingType, members);
        return true;
    }

    //This is a record because they have equality on all members. That's needed for incremental source generators.
    private readonly record struct CompilationInfo(INamedTypeSymbol? EnumAttr, INamedTypeSymbol? DisplayAttr, INamedTypeSymbol? FlagsAttr);
}