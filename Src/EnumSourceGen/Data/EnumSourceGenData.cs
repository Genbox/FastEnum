namespace Genbox.EnumSourceGen.Data;

internal class EnumSourceGenData
{
    public string? ExtensionClassName { get; set; }
    public string? ExtensionClassNamespace { get; set; }
    public string? EnumsClassName { get; set; }
    public string? EnumsClassNamespace { get; set; }
    public string? EnumNameOverride { get; set; }
    public bool DisableEnumsWrapper { get; set; }
    public bool DisableCache { get; set; }
}