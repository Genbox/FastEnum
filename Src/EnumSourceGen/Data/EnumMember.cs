namespace Genbox.EnumSourceGen.Data;

internal record EnumMember(string Name, object Value, DisplayData? DisplayData, EnumOmitValueData? OmitValueData, EnumTransformValueData? TransformValueData);