using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Genbox.FastEnum.Generators;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
#if RELEASE
using System.Globalization;
#endif

namespace Genbox.FastEnum;

/// <summary>Generates optimized helper APIs for enums marked with <see cref="FastEnumAttribute"/>.</summary>
[Generator(LanguageNames.CSharp)]
public class EnumGenerator : IIncrementalGenerator
{
    private const string DisplayAttribute = "System.ComponentModel.DataAnnotations.DisplayAttribute";
    private const string FlagsAttribute = "System.FlagsAttribute";
    private const string FastEnumAttr = "Genbox.FastEnum." + nameof(FastEnumAttribute);
    private const string EnumTransformAttr = "Genbox.FastEnum." + nameof(EnumTransformAttribute);
    private const string EnumTransformValueAttr = "Genbox.FastEnum." + nameof(EnumTransformValueAttribute);
    private const string EnumOmitValueAttr = "Genbox.FastEnum." + nameof(EnumOmitValueAttribute);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<EnumSpec>> sp = context.SyntaxProvider
                                                                       .ForAttributeWithMetadataName(FastEnumAttr, static (node, _) => node is EnumDeclarationSyntax m && m.AttributeLists.Count > 0, Transform)
                                                                       .Where(x => x != null)
                                                                       .Collect()!;

        AssemblyName name = GetType().Assembly.GetName();

        context.RegisterSourceOutput(sp, (spc, specs) =>
        {
            if (spc.CancellationToken.IsCancellationRequested)
                return;

            if (!IsSpecsValid(specs, out string? message))
            {
                DiagnosticDescriptor report = new DiagnosticDescriptor("FE001", "FastEnum", $"Validation failed with message: {message}", "errors", DiagnosticSeverity.Error, true);
                spc.ReportDiagnostic(Diagnostic.Create(report, Location.None));
                return;
            }

            foreach (EnumSpec enumSpec in specs)
            {
                try
                {
                    bool wrapperPublic = IsEnumsClassPublic(specs, enumSpec);

                    StringBuilder sb = new StringBuilder(4096);
                    spc.AddSource(enumSpec.FullName + "_EnumFormat.g.cs", GetSource(sb, name, enumSpec, EnumFormatCode.Generate));
                    spc.AddSource(enumSpec.FullName + "_Enums.g.cs", GetSource(sb, name, enumSpec, spec => EnumClassCode.Generate(spec, wrapperPublic)));
                    spc.AddSource(enumSpec.FullName + "_Extensions.g.cs", GetSource(sb, name, enumSpec, EnumExtensionCode.Generate));
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
        foreach (EnumSpec es in specs)
        {
            if (es.HasGenericContainingType)
            {
                message = $"FastEnum is not supported on enum '{es.FullName}' inside a generic containing type";
                return false;
            }
        }

        if (!AreGeneratedNamesValid(specs, out message))
            return false;

        return AreVisibilitiesValid(specs, out message);
    }

    private static bool AreGeneratedNamesValid(ImmutableArray<EnumSpec> specs, out string? message)
    {
        // By default, the enum helper is generated as <enum_namespace>.<enums_class_name>.<enum_name>.
        // <enum_namespace> is the namespace of the user's enum. It can be overridden by EnumsClassNamespace.
        // <enums_class_name> defaults to "Enums". It can be overridden by EnumsClassName.
        // <enum_name> is the name of the user's enum. It can be overridden by EnumNameOverride.
        //
        // The format enum and extension class also derive their names from these inputs. Combine the semantic names
        // of every generated type and check for duplicates so escaped identifiers cannot hide a collision.
        Dictionary<string, string> emittedTypes = new Dictionary<string, string>(StringComparer.Ordinal); // Case-sensitive since C# is too.

        foreach (EnumSpec es in specs)
        {
            FastEnumData esd = es.Data;

            // Validate user-provided identifiers before they are emitted into generated C#.
            if (!IsValidIdentifierOverride(esd.EnumNameOverride, nameof(FastEnumAttribute.EnumNameOverride), out message) ||
                !IsValidIdentifierOverride(esd.EnumsClassName, nameof(FastEnumAttribute.EnumsClassName), out message) ||
                !IsValidIdentifierOverride(esd.ExtensionClassName, nameof(FastEnumAttribute.ExtensionClassName), out message))
                return false;

            // Namespace overrides are also code, so reject invalid qualified names early.
            if (!IsValidNamespaceOverride(esd.EnumsClassNamespace, nameof(FastEnumAttribute.EnumsClassNamespace), out message) ||
                !IsValidNamespaceOverride(esd.ExtensionClassNamespace, nameof(FastEnumAttribute.ExtensionClassNamespace), out message))
                return false;

            string? enumNamespace = esd.EnumsClassNamespace ?? es.Namespace;
            string enumClassName = esd.EnumsClassName ?? "Enums";
            string enumName = es.Name;
            string? extensionNamespace = esd.ExtensionClassNamespace ?? es.Namespace;
            string extensionName = esd.ExtensionClassName ?? enumName + "Extensions";

            // Validate every emitted type using semantic identifiers; Foo and @Foo name the same type.
            if ((!esd.DisableEnumsWrapper && !AddEmittedType(JoinName(enumNamespace, enumClassName), "wrapper", true, out message)) ||
                !AddEmittedType(esd.DisableEnumsWrapper ? JoinName(enumNamespace, enumName) : JoinName(enumNamespace, enumClassName, enumName), "enum helper", false, out message) ||
                !AddEmittedType(JoinName(enumNamespace, enumName + "Format"), "format enum", false, out message) ||
                !AddEmittedType(JoinName(extensionNamespace, extensionName), "extension class", false, out message))
                return false;
        }

        message = null;
        return true;

        bool AddEmittedType(string fullName, string kind, bool allowSharedWrapper, out string? error)
        {
            if (!emittedTypes.TryGetValue(fullName, out string? existingKind))
            {
                emittedTypes.Add(fullName, kind);
                error = null;
                return true;
            }

            if (allowSharedWrapper && existingKind == "wrapper")
            {
                error = null;
                return true;
            }

            error = $"Generated {kind} collides with generated {existingKind}: {fullName}. Use a FastEnum name or namespace override to resolve the conflict";
            return false;
        }
    }

    private static bool AreVisibilitiesValid(ImmutableArray<EnumSpec> specs, out string? message)
    {
        // We don't support private enums. For example:
        //
        // public class MyClass
        // {
        //     private enum MyEnum { Value }
        // }
        //
        // The generated Enums.MyEnum class cannot expose the enum because it is private. Disabling the Enums wrapper
        // does not help because the resulting MyEnum class and generated extension methods still cannot expose it.
        // We therefore only support internal and public enums, and a containing type cannot be less visible than its enum.
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

            //Now we need to satisfy C#'s invariant: parents must have equal or more visibility than it's children
            if (es.AccessChain.Length > 1)
            {
                Accessibility parentAccess = es.AccessChain[1];

                if (parentAccess < enumAccess)
                {
                    message = $"Parent class is less visible ({parentAccess}) than enum '{es.FullName} ({enumAccess}). That is not supported";
                    return false;
                }
            }

            FastEnumData data = es.Data;

            if (data.EnumsClassVisibility != Visibility.Inherit && enumAccess <= Accessibility.Internal && data.EnumsClassVisibility == Visibility.Public)
            {
                message = $"Your visibility override ({data.EnumsClassVisibility}) on the enums class must be less or equal to the visibility on the enum '{es.FullName} ({enumAccess})";
                return false;
            }

            if (data.ExtensionClassVisibility != Visibility.Inherit && enumAccess <= Accessibility.Internal && data.ExtensionClassVisibility == Visibility.Public)
            {
                message = $"Your visibility override ({data.ExtensionClassVisibility}) on the extensions class must be less or equal to the visibility on the enum '{es.FullName} ({enumAccess})";
                return false;
            }
        }

        message = null;
        return true;
    }

    private static string JoinName(string? @namespace, params string[] names)
    {
        string name = string.Join(".", Array.ConvertAll(names, NormalizeIdentifier));

        return @namespace == null ? name : NormalizeQualifiedName(@namespace) + "." + name;
    }

    private static string NormalizeQualifiedName(string value)
    {
        return string.Join(".", Array.ConvertAll(value.Split('.'), NormalizeIdentifier));
    }

    private static string NormalizeIdentifier(string value) => value.Length > 0 && value[0] == '@' ? value.Substring(1) : value;

    private static bool IsEnumsClassPublic(ImmutableArray<EnumSpec> specs, EnumSpec target)
    {
        FastEnumData targetData = target.Data;
        string targetWrapper = JoinName(targetData.EnumsClassNamespace ?? target.Namespace, targetData.EnumsClassName ?? "Enums");

        // Group partial wrappers by semantic identity so Foo and @Foo share accessibility.
        foreach (EnumSpec spec in specs)
        {
            FastEnumData data = spec.Data;
            string wrapper = JoinName(data.EnumsClassNamespace ?? spec.Namespace, data.EnumsClassName ?? "Enums");
            if (data.DisableEnumsWrapper || wrapper != targetWrapper)
                continue;

            if (data.EnumsClassVisibility == Visibility.Public || (data.EnumsClassVisibility == Visibility.Inherit && spec.AccessChain[0] == Accessibility.Public))
                return true;
        }

        return false;
    }

    private static bool IsValidIdentifierOverride(string? value, string propertyName, out string? message)
    {
        if (value == null || IsValidIdentifier(value))
        {
            message = null;
            return true;
        }

        message = $"Invalid C# identifier '{value}' in {propertyName}";
        return false;
    }

    private static bool IsValidNamespaceOverride(string? value, string propertyName, out string? message)
    {
        if (value == null)
        {
            message = null;
            return true;
        }

        string[] parts = value.Split('.');
        if (parts.Length > 0 && Array.TrueForAll(parts, IsValidIdentifier))
        {
            message = null;
            return true;
        }

        message = $"Invalid C# namespace '{value}' in {propertyName}";
        return false;
    }

    private static bool IsValidIdentifier(string value)
    {
        if (SyntaxFacts.IsValidIdentifier(value))
            return true;

        // Overrides may retain an explicit escape even when the semantic identifier is ordinary.
        if (value.Length < 2 || value[0] != '@')
            return false;

        string unescaped = value.Substring(1);
        return SyntaxFacts.IsValidIdentifier(unescaped) || SyntaxFacts.GetKeywordKind(unescaped) != SyntaxKind.None || SyntaxFacts.GetContextualKeywordKind(unescaped) != SyntaxKind.None;
    }

    [SuppressMessage("Roslynator", "RCS1163:Unused parameter", Justification = "The parameter is used in release builds")]
    private static SourceText GetSource(StringBuilder sb, AssemblyName assemblyName, EnumSpec spec, Func<EnumSpec, string> action)
    {
        sb.Clear();
        sb.AppendLine("// <auto-generated />");

#if RELEASE
        sb.Append("// Generated by ").Append(assemblyName.Name).Append(' ').AppendLine(assemblyName.Version.ToString());
        sb.Append("// Generated on: ").AppendFormat(DateTimeFormatInfo.InvariantInfo, "{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow).AppendLine(" UTC");
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

                    hasName |= displayData.Name != null;
                    hasDescription |= displayData.Description != null;
                }
                else if (name.Equals(EnumTransformValueAttr, StringComparison.Ordinal))
                    transformValueData = TypeHelper.MapData<EnumTransformValueData>(ad.NamedArguments);
                else if (name.Equals(EnumOmitValueAttr, StringComparison.Ordinal))
                {
                    // If no arguments are given, we default to exclude all
                    if (ad.NamedArguments.Length == 0)
                        omitValueData = new EnumOmitValueData { Exclude = EnumOmitExclude.All };
                    else
                        omitValueData = TypeHelper.MapData<EnumOmitValueData>(ad.NamedArguments);
                }
            }

            string memberName = member.Name;
            members.Add(new EnumMemberSpec(memberName, EscapeIdentifier(memberName), field.ConstantValue, displayData, omitValueData, transformValueData));
        }

        // Underlying framework type names must not bind to user-defined symbols.
        string underlyingType = symbol.EnumUnderlyingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "global::System.Int32";

        List<Accessibility> accessChain = new List<Accessibility>();

        ISymbol? curSym = symbol;

        while (curSym != null)
        {
            accessChain.Add(curSym.DeclaredAccessibility);
            curSym = curSym.ContainingSymbol;
        }

        string enumName = NormalizeIdentifier(fastEnumData.EnumNameOverride ?? symbol.Name);
        // Symbol display formats handle containing types; only code references retain escapes and the global qualifier.
        string enumFullName = symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat).Replace("@", "");
        string fqn = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string? enumNamespace = symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString();
        bool hasGenericContainingType = false;

        for (INamedTypeSymbol? containingType = symbol.ContainingType; containingType != null; containingType = containingType.ContainingType)
            hasGenericContainingType |= containingType.Arity > 0;

        return new EnumSpec(enumName, EscapeIdentifier(enumName), enumFullName, fqn, enumNamespace, accessChain.ToArray(), hasGenericContainingType, hasName, hasDescription, hasFlags, underlyingType, fastEnumData, members.ToArray(), enumTransformData);
    }

    private static string EscapeIdentifier(string value)
    {
        // Roslyn symbol names omit the source '@', so restore it for keyword identifiers.
        return SyntaxFacts.GetKeywordKind(value) != SyntaxKind.None || SyntaxFacts.GetContextualKeywordKind(value) != SyntaxKind.None ? "@" + value : value;
    }
}