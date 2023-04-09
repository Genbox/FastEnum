namespace Genbox.EnumSourceGen.Data;

internal record EnumSpec(string Name, string FullName, string FullyQualifiedName, string? Namespace, bool IsPublic, bool HasDisplay, bool HasDescription, bool HasFlags, string UnderlyingType, EnumSourceGenData SourceGenData, List<EnumMember> Members, EnumTransformData? TransformData);