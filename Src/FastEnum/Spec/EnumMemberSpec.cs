using Genbox.FastEnum.Data;

namespace Genbox.FastEnum.Spec;

internal record EnumMemberSpec(string Name, object Value, DisplayData? DisplayData, EnumOmitValueData? OmitValueData, EnumTransformValueData? TransformValueData);