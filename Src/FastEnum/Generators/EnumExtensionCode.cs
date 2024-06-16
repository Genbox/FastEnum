using System.Text;
using Genbox.FastEnum.Data;
using Genbox.FastEnum.Helpers;
using Genbox.FastEnum.Spec;
using Microsoft.CodeAnalysis;
using static Genbox.FastEnum.Helpers.CodeGenHelper;

namespace Genbox.FastEnum.Generators;

internal static class EnumExtensionCode
{
    public static string Generate(EnumSpec es)
    {
        FastEnumData op = es.Data;

        string? ns = op.ExtensionClassNamespace ?? es.Namespace;
        string cn = op.EnumNameOverride ?? es.Name;
        string en = op.ExtensionClassName ?? cn + "Extensions";
        string sn = es.Namespace == null ? "global::" + es.FullyQualifiedName : es.FullyQualifiedName;
        string vi = op.ExtensionClassVisibility == Visibility.Inherit ? (es.AccessChain[0] == Accessibility.Public ? "public" : "internal") : op.ExtensionClassVisibility.ToString().ToLowerInvariant();
        string ut = es.UnderlyingType;

        bool containsDuplicateValue = false;
        HashSet<object> values = new HashSet<object>();

        foreach (var em in es.Members)
        {
            if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.GetString) == true)
                continue;

            if (!values.Add(em.Value))
            {
                containsDuplicateValue = true;
                break;
            }
        }

        StringBuilder sb = new StringBuilder();
        string res = $$"""
                       using System;
                       using System.Diagnostics.CodeAnalysis;
                       {{(ns != null ? "\nnamespace " + ns + ";\n" : null)}}
                       {{vi}} static partial class {{en}}
                       {
                           public static string GetString(this {{sn}} value) => {{(containsDuplicateValue ? "value.ToString();" : $"value switch\n    {{\n        {GetString()}\n        _ => value.ToString()\n    }};")}}

                           public static bool TryGetUnderlyingValue(this {{sn}} value, out {{ut}} underlyingValue)
                           {
                               {{PrintSwitch(TryGetUnderlyingValue(), containsDuplicateValue)}}
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


                         public static bool TryGetDisplayName(this {{sn}} value,
                     #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                     [NotNullWhen(true)]
                     #endif
                     out string? displayName)
                         {
                             {{PrintSwitch(TryGetDisplayName())}}
                             displayName = null;
                             return false;
                         }

                         public static string GetDisplayName(this {{sn}} value)
                         {
                             if (!TryGetDisplayName(value, out string? displayName))
                                 throw new ArgumentOutOfRangeException($"Invalid value: {value}");

                             return displayName!;
                         }
                     """;
        }

        if (es.HasDescription)
        {
            res += $$"""


                         public static bool TryGetDescription(this {{sn}} value,
                     #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
                     [NotNullWhen(true)]
                     #endif
                     out string? description)
                         {
                             {{PrintSwitch(TryGetDescription())}}
                             description = null;
                             return false;
                         }

                         public static string GetDescription(this {{sn}} value)
                         {
                             if (!TryGetDescription(value, out string? description))
                                 throw new ArgumentOutOfRangeException($"Invalid value: {value}");

                             return description!;
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

                sb.Append(sn).Append('.').Append(em.Name).Append(" => \"").Append(transformed)
                  .Append("\",\n        ");
            }

            return sb.ToString().TrimEnd();
        }

        IEnumerable<string> TryGetUnderlyingValue()
        {
            foreach (EnumMemberSpec em in es.Members)
            {
                if (em.OmitValueData?.Exclude.HasFlag(EnumOmitExclude.TryGetUnderlyingValue) == false)
                    continue;

                //We default to doing a fast comparison using enum values (which is basically just integers), but in the case we have a flags enum with a duplicate value
                //we must fall back to using string comparisons, otherwise there will be duplicate branches in the switch.
                if (containsDuplicateValue)
                {
                    yield return $$"""
                                               case "{{em.Name}}":
                                                   underlyingValue = {{em.Value}};
                                                   return true;
                                   """;
                }
                else
                {
                    yield return $$"""
                                               case {{sn}}.{{em.Name}}:
                                                   underlyingValue = {{em.Value}};
                                                   return true;
                                   """;
                }
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

        string PrintSwitch(IEnumerable<string> cases, bool stringComparison = false)
        {
            string[] arr = cases.ToArray();

            if (arr.Length == 0)
                return string.Empty;

            sb.Clear();

            if (stringComparison)
                sb.AppendLine("switch (value.ToString())");
            else
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