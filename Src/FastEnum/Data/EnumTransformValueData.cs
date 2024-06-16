using JetBrains.Annotations;

namespace Genbox.FastEnum.Data;

internal record EnumTransformValueData
{
    public string ValueOverride { get; [UsedImplicitly]set; }
}