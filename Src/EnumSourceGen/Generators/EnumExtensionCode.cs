using System.Text;
using Genbox.EnumSourceGen.Data;
using Genbox.EnumSourceGen.Helpers;
using Genbox.EnumSourceGen.Spec;
using static Genbox.EnumSourceGen.Helpers.CodeGenHelper;

namespace Genbox.EnumSourceGen.Generators;

internal static class EnumExtensionCode
{
    public static string Generate(EnumSpec es, StringBuilder sb)
    {
        sb.Clear();

        EnumSourceGenData op = es.SourceGenData;

        string? ns = op.ExtensionClassNamespace ?? es.Namespace;
        string cn = op.EnumNameOverride ?? es.Name;
        string en = op.ExtensionClassName ?? cn + "Extensions";
        string sn = es.Namespace == null ? "global::" + es.FullyQualifiedName : es.FullyQualifiedName;
        string vi = es.IsPublic ? "public" : "internal";
        string ut = es.UnderlyingType;

        string res = $$"""
using System;
using System.Diagnostics.CodeAnalysis;
{{(ns != null ? "\nnamespace " + ns + ";\n" : null)}}
{{vi}} static partial class {{en}}
{
    public static string GetString(this {{sn}} value)
        => value switch
        {
            {{GetString()}}
            _ => value.ToString()
        };

    public static bool TryGetUnderlyingValue(this {{sn}} value, out {{ut}} underlyingValue)
    {
        {{PrintSwitch(TryGetUnderlyingValue())}}
        underlyingValue = default;
        return false;
    }

    public static {{ut}} GetUnderlyingValue(this {{sn}} value)
    {
        if (!TryGetUnderlyingValue(value, out {{ut}} underlyingValue))
            throw new ArgumentOutOfRangeException($"Invalid value: {value}");

        return underlyingValue;
    }
""";

        if (es.HasDisplay)
        {
            res += $$"""


    public static bool TryGetDisplayName(this {{sn}} value, [NotNullWhen(true)]out string? displayName)
    {
        {{PrintSwitch(TryGetDisplayName())}}
        displayName = null;
        return false;
    }

    public static string GetDisplayName(this {{sn}} value)
    {
        if (!TryGetDisplayName(value, out string? displayName))
            throw new ArgumentOutOfRangeException($"Invalid value: {value}");

        return displayName;
    }
""";
        }

        if (es.HasDescription)
        {
            res += $$"""


    public static bool TryGetDescription(this {{sn}} value, [NotNullWhen(true)]out string? description)
    {
        {{PrintSwitch(TryGetDescription())}}
        description = null;
        return false;
    }

    public static string GetDescription(this {{sn}} value)
    {
        if (!TryGetDescription(value, out string? description))
            throw new ArgumentOutOfRangeException($"Invalid value: {value}");

        return description;
    }
""";
        }

        if (es.HasFlags)
        {
            res += $$"""


    public static bool IsFlagSet(this {{sn}} value, {{sn}} flag) => (({{ut}})value & ({{ut}})flag) == ({{ut}})flag;
""";
        }

        string GetString()
        {
            sb.Clear();

            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == false)
                {
                    sb.Append(sn).Append('.').Append(em.Name).Append(" => string.Empty,\n            ");
                    continue;
                }

                string transformed = TransformHelper.TransformName(es, em);

                sb.Append(sn).Append('.').Append(em.Name).Append(" => \"").Append(transformed).Append("\",\n            ");
            }

            return sb.ToString().TrimEnd();
        }

        IEnumerable<string> TryGetUnderlyingValue()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) == false)
                    continue;

                yield return $$"""
            case {{sn}}.{{em.Name}}:
                underlyingValue = {{em.Value}};
                return true;
""";
            }
        }

        IEnumerable<string> TryGetDisplayName()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetDisplayName) == false)
                    continue;

                if (em.DisplayData?.Name == null)
                    continue;

                yield return $$"""
            case {{sn}}.{{em.Name}}:
                displayName = "{{em.DisplayData.Name}}";
                return true;
""";
            }
        }

        IEnumerable<string> TryGetDescription()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetDescription) == false)
                    continue;

                if (em.DisplayData?.Description == null)
                    continue;

                yield return $$"""
            case {{sn}}.{{em.Name}}:
                description = "{{em.DisplayData.Description}}";
                return true;
""";
            }
        }

        string PrintSwitch(IEnumerable<string> cases)
        {
            string[] arr = cases.ToArray();

            if (arr.Length == 0)
                return string.Empty;

            sb.Clear();
            sb.AppendLine("switch (value)");
            sb.Append(Indent(2)).Append('{');
            sb.AppendLine();

            for (int i = 0; i < arr.Length; i++)
            {
                sb.Append(arr[i]);

                if (i != arr.Length - 1)
                    sb.AppendLine();
            }

            sb.AppendLine();
            sb.Append(Indent(2)).Append('}');

            return sb.ToString();
        }

        return res + "\n}";
    }
}