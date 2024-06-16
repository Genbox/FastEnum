namespace Genbox.FastEnum.Data;

internal record EnumMemberSpec(string Name, object Value, DisplayData? DisplayData, EnumOmitValueData? OmitValueData, EnumTransformValueData? TransformValueData);