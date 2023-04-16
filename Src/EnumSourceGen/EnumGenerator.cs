using System.Collections.Immutable;
using System.Text;
using Genbox.EnumSourceGen.Data;
using Genbox.EnumSourceGen.Generators;
using Genbox.EnumSourceGen.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Genbox.EnumSourceGen;

[Generator]
public class EnumGenerator : IIncrementalGenerator
{
    private const string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";
    private const string FlagsAttribute = "System.FlagsAttribute";
    private const string EnumSourceGenAttr = "Genbox.EnumSourceGen." + nameof(EnumSourceGenAttribute);
    private const string EnumTransformAttr = "Genbox.EnumSourceGen." + nameof(EnumTransformAttribute);
    private const string EnumTransformValueAttr = "Genbox.EnumSourceGen." + nameof(EnumTransformValueAttribute);
    private const string EnumOmitValueAttr = "Genbox.EnumSourceGen." + nameof(EnumOmitValueAttribute);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<CompilationInfo> cp = context.CompilationProvider
                                                              .Select(static (c, _) => new CompilationInfo(
                                                                  c.GetTypeByMetadataName(EnumSourceGenAttr),
                                                                  c.GetTypeByMetadataName(DisplayAttribute),
                                                                  c.GetTypeByMetadataName(FlagsAttribute),
                                                                  c.GetTypeByMetadataName(EnumTransformAttr),
                                                                  c.GetTypeByMetadataName(EnumTransformValueAttr),
                                                                  c.GetTypeByMetadataName(EnumOmitValueAttr)));

        IncrementalValuesProvider<ISymbol?> sp = context.SyntaxProvider
                                                        .ForAttributeWithMetadataName(EnumSourceGenAttr, static (node, _) => node is EnumDeclarationSyntax m && m.AttributeLists.Count > 0, Transform)
                                                        .Where(static x => x is not null);

        context.RegisterSourceOutput(sp.Combine(cp), static (spc, source) =>
        {
            if (!TryGetTypesToGenerate(source.Right, source.Left, out EnumSpec? es))
                return;

            if (es == null)
                return;

            try
            {
                StringBuilder sb = new StringBuilder();

                string fqn = es.FullyQualifiedName;

                spc.AddSource(fqn + "_EnumFormat.g.cs", SourceText.From(EnumFormatCode.Generate(es), Encoding.UTF8));
                spc.AddSource(fqn + "_Enums.g.cs", SourceText.From(EnumClassCode.Generate(es, sb), Encoding.UTF8));
                spc.AddSource(fqn + "_Extensions.g.cs", SourceText.From(EnumExtensionCode.Generate(es, sb), Encoding.UTF8));
            }
            catch (Exception e)
            {
                DiagnosticDescriptor report = new DiagnosticDescriptor("ESG001", "EnumSourceGen", $"An error happened while generating code for {es.FullName}. Error: {e.Message}", "errors", DiagnosticSeverity.Error, true);
                spc.ReportDiagnostic(Diagnostic.Create(report, Location.None));
            }
        });
    }

    private static ISymbol? Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        EnumDeclarationSyntax syntax = (EnumDeclarationSyntax)context.TargetNode;

        foreach (AttributeListSyntax attrList in syntax.AttributeLists)
        {
            foreach (AttributeSyntax attrSyntax in attrList.Attributes)
            {
                SymbolInfo symbolInfo = context.SemanticModel.GetSymbolInfo(attrSyntax, token);

                if (symbolInfo.Symbol == null)
                    throw new InvalidCastException("BUG");

                string fullName = symbolInfo.Symbol.ContainingType.ToDisplayString();

                if (string.Equals(fullName, EnumSourceGenAttr, StringComparison.Ordinal))
                    return context.SemanticModel.GetDeclaredSymbol(syntax, token);
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    private static bool TryGetTypesToGenerate(CompilationInfo info, ISymbol? symbol, out EnumSpec? enumSpec)
    {
        // nothing to do if this type isn't available
        if (info.EnumAttr == null)
        {
            enumSpec = null;
            return false;
        }

        if (symbol is not INamedTypeSymbol enumSymbol)
            throw new InvalidOperationException("BUG");

        INamedTypeSymbol? enumAttr = info.EnumAttr;
        INamedTypeSymbol? flagsAttr = info.FlagsAttr;
        INamedTypeSymbol? transformAttr = info.TransformAttr;

        bool hasFlags = false;
        EnumSourceGenData? enumSourceGenData = null;
        EnumTransformData? enumTransformData = null;

        foreach (AttributeData ad in enumSymbol.GetAttributes())
        {
            if (enumAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                enumSourceGenData = TypeHelper.MapData<EnumSourceGenData>(ad.NamedArguments);
            else if (flagsAttr != null && flagsAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                hasFlags = true;
            else if (transformAttr != null && transformAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                enumTransformData = TypeHelper.MapData<EnumTransformData>(ad.NamedArguments);
        }

        if (enumSourceGenData == null)
            throw new InvalidCastException("Was unable to read attribute data");

        //Now we read attributes applied to members of the enum
        ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
        List<EnumMember> members = new List<EnumMember>(enumMembers.Length);

        INamedTypeSymbol? displayAttr = info.DisplayAttr;
        INamedTypeSymbol? transformValueAttr = info.TransformValueAttr;
        INamedTypeSymbol? omitAttr = info.OmitAttr;

        bool hasName = false;
        bool hasDescription = false;

        foreach (ISymbol member in enumMembers)
        {
            if (member is not IFieldSymbol field || field.ConstantValue == null)
                continue;

            DisplayData? displayData = null;
            EnumTransformValueData? transformValueData = null;
            EnumOmitValueData? omitValueData = null;

            if (displayAttr != null || transformValueAttr != null || omitAttr != null)
            {
                foreach (AttributeData? ad in field.GetAttributes())
                {
                    if (displayAttr != null && displayAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                    {
                        displayData = TypeHelper.MapData<DisplayData>(ad.NamedArguments);

                        hasName = displayData.Name != null;
                        hasDescription = displayData.Description != null;
                    }
                    else if (transformValueAttr != null && transformValueAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                        transformValueData = TypeHelper.MapData<EnumTransformValueData>(ad.NamedArguments);
                    else if (omitAttr != null && omitAttr.Equals(ad.AttributeClass, SymbolEqualityComparer.Default))
                        omitValueData = TypeHelper.MapData<EnumOmitValueData>(ad.NamedArguments);
                }
            }

            members.Add(new EnumMember(member.Name, field.ConstantValue, displayData, omitValueData, transformValueData));
        }

        string underlyingType = enumSymbol.EnumUnderlyingType?.Name ?? "int";
        bool isPublic = enumSymbol.DeclaredAccessibility == Accessibility.Public;

        ImmutableArray<SymbolDisplayPart> parts = enumSymbol.ToDisplayParts();

        StringBuilder fqnSb = new StringBuilder(50);
        StringBuilder namespaceSb = new StringBuilder(25);
        StringBuilder enumFullSb = new StringBuilder(25);

        bool inNamespace = false;
        foreach (SymbolDisplayPart part in parts)
        {
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

        enumSpec = new EnumSpec(enumName, enumFullName, fqn, enumNamespace, isPublic, hasName, hasDescription, hasFlags, underlyingType, enumSourceGenData, members, enumTransformData);
        return true;
    }

    //This is a record because they have equality on all members. That's needed for incremental source generators.
    private readonly record struct CompilationInfo(INamedTypeSymbol? EnumAttr, INamedTypeSymbol? DisplayAttr, INamedTypeSymbol? FlagsAttr, INamedTypeSymbol? TransformAttr, INamedTypeSymbol? TransformValueAttr, INamedTypeSymbol? OmitAttr);
}