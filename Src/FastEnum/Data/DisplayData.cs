using JetBrains.Annotations;

namespace Genbox.FastEnum.Data;

internal record DisplayData
{
    public string? Name { get; [UsedImplicitly]set; }
    public string? Description { get; [UsedImplicitly]set; }
}