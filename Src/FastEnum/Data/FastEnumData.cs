using JetBrains.Annotations;

namespace Genbox.FastEnum.Data;

internal record FastEnumData
{
    public string? ExtensionClassName { get; [UsedImplicitly]set; }
    public string? ExtensionClassNamespace { get; [UsedImplicitly]set; }
    public Visibility ExtensionClassVisibility { get; [UsedImplicitly]set; }
    public string? EnumsClassName { get; [UsedImplicitly]set; }
    public string? EnumsClassNamespace { get; [UsedImplicitly]set; }
    public Visibility EnumsClassVisibility { get; [UsedImplicitly]set; }
    public string? EnumNameOverride { get; [UsedImplicitly]set; }
    public bool DisableEnumsWrapper { get; [UsedImplicitly]set; }
    public bool DisableCache { get; [UsedImplicitly]set; }
}