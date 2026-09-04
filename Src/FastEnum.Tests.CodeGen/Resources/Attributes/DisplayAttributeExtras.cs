// Extra DisplayAttribute properties are ignored; literal text survives escaping and parsing.

[FastEnum]
public enum DisplayExtras
{
    [Display(Name = "First\u0085Second\u2028Third\u2029Fourth", Description = "Details\u2029Next", ShortName = "Short", Order = 1, GroupName = "Group", Prompt = "Prompt", AutoGenerateField = false, AutoGenerateFilter = true)]
    Value,
    [Display(Name = "Label", ResourceType = typeof(DisplayResources), ShortName = null)]
    ResourceKey
}

public static class DisplayResources
{
    public static string Label => "Label";
}