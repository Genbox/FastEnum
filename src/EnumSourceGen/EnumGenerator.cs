using System.Collections.Immutable;
using System.Text;
using Genbox.EnumSourceGen.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Genbox.EnumSourceGen;

internal readonly record struct AttributeOptions(Generate Generate, string? EnumsClassName, string? EnumsClassNamespace, string? ExtensionsName, string? ExtensionsNamespace, string? EnumNameOverride);
internal readonly record struct EnumSpec(string Name, string FullName, string FullyQualifiedName, string? Namespace, bool IsPublic, bool HasDisplay, bool HasDescription, bool HasFlags, string UnderlyingType, List<EnumMember> Members);
internal readonly record struct EnumMember(string Name, object? Value, string? DisplayName, string? Description);

[Generator]
public class EnumGenerator : IIncrementalGenerator
{
    private const string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";
    private const string FlagsAttribute = "System.FlagsAttribute";
    private const string EnumExtensionsAttribute = "Genbox.EnumSourceGen.EnumSourceGenAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<CompilationInfo> cp = context.CompilationProvider
                                                              .Select(static (c, _) => new CompilationInfo(c.GetTypeByMetadataName(EnumExtensionsAttribute), c.GetTypeByMetadataName(DisplayAttribute), c.GetTypeByMetadataName(FlagsAttribute)));

        IncrementalValuesProvider<ISymbol> sp = context.SyntaxProvider
                                                       .CreateSyntaxProvider(Predicate, Transform)
                                                       .Where(s => s != null)!;

        context.RegisterSourceOutput(sp.Combine(cp), (spc, source) =>
        {
            if (!TryGetTypesToGenerate(source.Right, source.Left, out EnumSpec es, out AttributeOptions op))
                return;

            StringBuilder sb = new StringBuilder();

            string fqn = es.FullyQualifiedName;

            switch (op.Generate)
            {
                case Generate.ClassAndExtensions:
                    spc.AddSource(fqn + "_EnumFormat.g.cs", SourceText.From(EnumFormatCode.Generate(es, op, sb), Encoding.UTF8));
                    spc.AddSource(fqn + "_Enums.g.cs", SourceText.From(EnumClassCode.Generate(es, op, sb), Encoding.UTF8));
                    spc.AddSource(fqn + "_Extensions.g.cs", SourceText.From(ExtensionCode.Generate(es, op, sb), Encoding.UTF8));
                    break;
                case Generate.ClassOnly:
                    spc.AddSource(fqn + "_EnumFormat.g.cs", SourceText.From(EnumFormatCode.Generate(es, op, sb), Encoding.UTF8));
                    spc.AddSource(fqn + "_Enums.g.cs", SourceText.From(EnumClassCode.Generate(es, op, sb), Encoding.UTF8));
                    break;
                case Generate.ExtensionsOnly:
                    spc.AddSource(fqn + "_Extensions.g.cs", SourceText.From(ExtensionCode.Generate(es, op, sb), Encoding.UTF8));
                    break;
                default:
                    throw new InvalidOperationException($"Value '{op.Generate}' is outside of supported values");
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

    private static bool TryGetTypesToGenerate(CompilationInfo info, ISymbol? symbol, out EnumSpec enumSpec, out AttributeOptions options)
    {
        // nothing to do if this type isn't available
        if (info.EnumAttr == null)
        {
            enumSpec = default;
            options = default;
            return false;
        }

        if (symbol is not INamedTypeSymbol enumSymbol)
            throw new InvalidOperationException("BUG");

        string? optExtensionsName = null;
        string? optExtensionsNamespace = null;
        string? optEnumsName = null;
        string? optEnumsNamespace = null;
        string? optEnumNameOverride = null;

        Generate optGenerate = Generate.ClassAndExtensions;

        INamedTypeSymbol? flagsAttr = info.FlagsAttr;
        bool hasFlags = false;

        ImmutableArray<AttributeData> attr = enumSymbol.GetAttributes();

        for (int i = 0; i < attr.Length; i++)
        {
            AttributeData ad = attr[i];

            //Check if the attribute is the FlagsAttribute
            if (flagsAttr != null && flagsAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
            {
                hasFlags = true;
                continue;
            }

            for (int j = 0; j < ad.NamedArguments.Length; j++)
            {
                KeyValuePair<string, TypedConstant> na = ad.NamedArguments[j];

                if (na.Value.Value == null)
                    continue;

                switch (na.Key)
                {
                    case nameof(EnumSourceGenAttribute.Generate):
                        optGenerate = (Generate)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.ExtensionClassName):
                        optExtensionsName = (string)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.ExtensionClassNamespace):
                        optExtensionsNamespace = (string?)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.EnumsClassName):
                        optEnumsName = (string)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.EnumsClassNamespace):
                        optEnumsNamespace = (string)na.Value.Value;
                        break;
                    case nameof(EnumSourceGenAttribute.EnumNameOverride):
                        optEnumNameOverride = (string)na.Value.Value;
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

        string enumName = enumSymbol.Name;
        string enumFullName = enumFullSb.ToString(); //This include the nested type name (if any)
        string fqn = fqnSb.ToString();
        string? enumNamespace = namespaceSb.Length == 0 ? null : namespaceSb.ToString().TrimEnd('.');

        options = new AttributeOptions(optGenerate, optEnumsName, optEnumsNamespace, optExtensionsName, optExtensionsNamespace, optEnumNameOverride);
        enumSpec = new EnumSpec(enumName, enumFullName, fqn, enumNamespace, isPublic, hasDisplay, hasDescription, hasFlags, underlyingType, members);
        return true;
    }

    //This is a record because they have equality on all members. That's needed for incremental source generators.
    private readonly record struct CompilationInfo(INamedTypeSymbol? EnumAttr, INamedTypeSymbol? DisplayAttr, INamedTypeSymbol? FlagsAttr);
}