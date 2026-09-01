namespace Genbox.FastEnum.Data;

internal record EnumMemberSpec(string Name, string EmittedIdentifier, object Value, DisplayData? DisplayData, EnumOmitValueData? OmitValueData, EnumTransformValueData? TransformValueData);