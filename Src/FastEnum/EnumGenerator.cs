using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Genbox.FastEnum.Data;
using Genbox.FastEnum.Generators;
using Genbox.FastEnum.Helpers;
using Genbox.FastEnum.Spec;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
#if RELEASE
using System.Globalization;
#endif

namespace Genbox.FastEnum;

[Generator(LanguageNames.CSharp)]
public class EnumGenerator : IIncrementalGenerator
{
    private const string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";
    private const string FlagsAttribute = "System.FlagsAttribute";
    private const string FastEnumAttr = "Genbox.FastEnum." + nameof(FastEnumAttribute);
    private const string EnumTransformAttr = "Genbox.FastEnum." + nameof(EnumTransformAttribute);
    private const string EnumTransformValueAttr = "Genbox.FastEnum." + nameof(EnumTransformValueAttribute);
    private const string EnumOmitValueAttr = "Genbox.FastEnum." + nameof(EnumOmitValueAttribute);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<EnumSpec>> sp = context.SyntaxProvider
                                                                       .ForAttributeWithMetadataName(FastEnumAttr, static (node, _) => node is EnumDeclarationSyntax m && m.AttributeLists.Count > 0, Transform)
                                                                       .Where(x => x != null)
                                                                       .Collect()!;

        AssemblyName name = GetType().Assembly.GetName();

        context.RegisterSourceOutput(sp, (spc, specs) =>
        {
            spc.CancellationToken.ThrowIfCancellationRequested();

            if (!IsSpecsValid(specs, out string? message))
            {
                DiagnosticDescriptor report = new DiagnosticDescriptor("ESG001", "FastEnum", $"Validation failed with message: {message}", "errors", DiagnosticSeverity.Error, true);
                spc.ReportDiagnostic(Diagnostic.Create(report, Location.None));
                return;
            }

            foreach (EnumSpec enumSpec in specs)
            {
                try
                {
                    string fqn = enumSpec.FullyQualifiedName;

                    StringBuilder sb = new StringBuilder();
                    spc.AddSource(fqn + "_EnumFormat.g.cs", GetSource(sb, name, enumSpec, EnumFormatCode.Generate));
                    spc.AddSource(fqn + "_Enums.g.cs", GetSource(sb, name, enumSpec, EnumClassCode.Generate));
                    spc.AddSource(fqn + "_Extensions.g.cs", GetSource(sb, name, enumSpec, EnumExtensionCode.Generate));
                }
                catch (Exception e)
                {
                    DiagnosticDescriptor report = new DiagnosticDescriptor("ESG002", "FastEnum", $"An error happened while generating code for {enumSpec.FullName}. Error: {e.Message}", "errors", DiagnosticSeverity.Error, true);
                    spc.ReportDiagnostic(Diagnostic.Create(report, Location.None));
                }
            }
        });
    }

    private static bool IsSpecsValid(ImmutableArray<EnumSpec> specs, out string? message)
    {
        //### Detect name conflicts ###
        // By default the Enums class is generated as: <enum_namespace>.<enums_class_name>.<enum_name>
        // <enum_namespace> is the namespace of the user's enum. It can be overriden by EnumsClassNamespace
        // <enum_class_name> defaults to "Enums". It can be overriden by EnumsClassName
        // <enum_name> is the name of the user's enum. It can be overriden by EnumNameOverride
        //
        // We therefore have to combine all these parts and check if there are duplicates.

        HashSet<string> nameSet = new HashSet<string>(StringComparer.Ordinal); //Case sensitive since C# is too

        foreach (EnumSpec es in specs)
        {
            FastEnumData esd = es.Data;

            string enumNamespace = esd.EnumsClassNamespace ?? (es.Namespace ?? "global::");
            string enumClassName = esd.EnumsClassName ?? "Enums";
            string enumName = esd.EnumNameOverride ?? es.Name;

            string fullName = string.Join(".", enumNamespace, enumClassName, enumName);

            if (!nameSet.Add(fullName))
            {
                message = $"Two enums collide in name: {fullName}. Use {nameof(FastEnumAttribute.EnumNameOverride)}, {nameof(FastEnumAttribute.EnumsClassName)} or {nameof(FastEnumAttribute.EnumsClassNamespace)} to resolve the conflict";
                return false;
            }
        }

        //### Detect accessibility/visibility issues ###
        // We don't support private enums. For example:
        //
        // public class MyClass
        // {
        //     private enum MyEnum { Value }
        // }
        //
        // The reason being that the generated Enums.MyEnum class can't expose the enum due to it being private.
        // The user can disable the Enums wrapper with DisableEnumsWrapper, but the resulting MyEnum class still can't expose the private enum.
        // We could stop generating the Enums.MyEnum class altogether, but the generated extension methods wouldn't work either. So then, what is the point?
        //
        // We therefore only support internal and public enums. However, we can't have a public enum in an internal class.

        foreach (EnumSpec es in specs)
        {
            //The first part of the AccessChain is the enum's own accessibility
            Accessibility enumAccess = es.AccessChain[0];

            if (enumAccess == Accessibility.Private)
            {
                message = $"FastEnum is not supported on private enum: '{es.FullName}'";
                return false;
            }

            if (enumAccess != Accessibility.Internal && enumAccess != Accessibility.Public)
            {
                message = $"Unsupported visibility '{enumAccess}' on '{es.FullName}'";
                return false;
            }

            //Now we need to satisfy C#'s invariant: parents must have equal or less accessibility than it's children
            //TODO
        }

        message = null;
        return true;
    }

    private static SourceText GetSource(StringBuilder sb, AssemblyName assemblyName, EnumSpec spec, Func<EnumSpec, string> action)
    {
        sb.Clear();
        sb.AppendLine("// <auto-generated />");

#if RELEASE
        sb.Append("//Generated by ").Append(assemblyName.Name).Append(' ').AppendLine(assemblyName.Version.ToString());
        sb.Append("//Generated on: ").AppendFormat(DateTimeFormatInfo.InvariantInfo, "{0:yyyy-MM-dd hh:mm:ss}", DateTime.UtcNow).AppendLine(" UTC)");
#endif

        sb.AppendLine("#nullable enable");
        sb.Append(action(spec));

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }

    private static EnumSpec? Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;

        bool hasFlags = false;
        FastEnumData? fastEnumData = null;
        EnumTransformData? enumTransformData = null;

        foreach (AttributeData ad in symbol.GetAttributes())
        {
            if (ad.AttributeClass == null)
                continue;

            string name = ad.AttributeClass.ToDisplayString();

            if (name.Equals(FastEnumAttr, StringComparison.Ordinal))
                fastEnumData = TypeHelper.MapData<FastEnumData>(ad.NamedArguments);
            else if (name.Equals(FlagsAttribute, StringComparison.Ordinal))
                hasFlags = true;
            else if (name.Equals(EnumTransformAttr, StringComparison.Ordinal))
                enumTransformData = TypeHelper.MapData<EnumTransformData>(ad.NamedArguments);
        }

        if (fastEnumData == null)
            return null;

        //Now we read attributes applied to members of the enum
        ImmutableArray<ISymbol> enumMembers = symbol.GetMembers();
        List<EnumMemberSpec> members = new List<EnumMemberSpec>(enumMembers.Length);

        bool hasName = false;
        bool hasDescription = false;

        foreach (ISymbol member in enumMembers)
        {
            if (member is not IFieldSymbol field || field.ConstantValue == null)
                continue;

            DisplayData? displayData = null;
            EnumTransformValueData? transformValueData = null;
            EnumOmitValueData? omitValueData = null;

            foreach (AttributeData ad in field.GetAttributes())
            {
                if (ad.AttributeClass == null)
                    continue;

                string name = ad.AttributeClass.ToDisplayString();

                if (name.Equals(DisplayAttribute, StringComparison.Ordinal))
                {
                    displayData = TypeHelper.MapData<DisplayData>(ad.NamedArguments);

                    hasName = displayData.Name != null;
                    hasDescription = displayData.Description != null;
                }
                else if (name.Equals(EnumTransformValueAttr, StringComparison.Ordinal))
                    transformValueData = TypeHelper.MapData<EnumTransformValueData>(ad.NamedArguments);
                else if (name.Equals(EnumOmitValueAttr, StringComparison.Ordinal))
                    omitValueData = TypeHelper.MapData<EnumOmitValueData>(ad.NamedArguments);
            }

            members.Add(new EnumMemberSpec(member.Name, field.ConstantValue, displayData, omitValueData, transformValueData));
        }

        string underlyingType = symbol.EnumUnderlyingType?.Name ?? "int";

        List<Accessibility> accessChain = new List<Accessibility>();

        ISymbol? curSym = symbol;

        while (curSym != null)
        {
            accessChain.Add(symbol.DeclaredAccessibility);
            curSym = curSym.ContainingSymbol;
        }

        ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts();

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
                default:
                    throw new InvalidOperationException("Unsupported value: " + part.Kind);
            }

            if (inNamespace)
                namespaceSb.Append(part);
            else
                enumFullSb.Append(part);

            fqnSb.Append(part);
        }

        string enumName = symbol.Name;
        string enumFullName = enumFullSb.ToString(); //This include the nested type name (if any)
        string fqn = fqnSb.ToString();
        string? enumNamespace = namespaceSb.Length == 0 ? null : namespaceSb.ToString().TrimEnd('.');

        return new EnumSpec(enumName, enumFullName, fqn, enumNamespace, accessChain.ToArray(), hasName, hasDescription, hasFlags, underlyingType, fastEnumData, members.ToArray(), enumTransformData);
    }
}