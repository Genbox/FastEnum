using JetBrains.Annotations;

namespace Genbox.FastEnum.Data;

internal record EnumOmitValueData
{
    public EnumOmitExclude Exclude { get; [UsedImplicitly]set; }
}