using JetBrains.Annotations;

namespace Genbox.FastEnum.Data;

internal record EnumTransformData
{
    public EnumTransform Preset { get; [UsedImplicitly]set; }
    public string? Regex { get; [UsedImplicitly]set; }
    public string? CasePattern { get; [UsedImplicitly]set; }
    public EnumOrder SortMemberNames { get; [UsedImplicitly]set; }
    public EnumOrder SortMemberValues { get; [UsedImplicitly]set; }
    public EnumOrder SortUnderlyingValues { get; [UsedImplicitly]set; }
    public EnumOrder SortDisplayNames { get; [UsedImplicitly]set; }
    public EnumOrder SortDescriptions { get; [UsedImplicitly]set; }
}