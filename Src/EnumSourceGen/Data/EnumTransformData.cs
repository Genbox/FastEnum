namespace Genbox.EnumSourceGen.Data;

internal class EnumTransformData
{
    public string? Transform { get; set; }
    public EnumOrder SortMemberNames { get; set; }
    public EnumOrder SortMemberValues { get; set; }
    public EnumOrder SortUnderlyingValues { get; set; }
    public EnumOrder SortDisplayNames { get; set; }
    public EnumOrder SortDescriptions { get; set; }
}