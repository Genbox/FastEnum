namespace Genbox.EnumSourceGen.Data;

internal class EnumTransformData
{
    // public void Validate(string name)
    // {
    //     int allowed = NameOverride != null ? 1 : 0;
    //     allowed += Preset != EnumTransform.None ? 1 : 0;
    //     allowed += Transform != null ? 1 : 0;
    //
    //     if (allowed > 1)
    //         throw new InvalidOperationException($"Multiple transforms (NameOverride, Simple, Advanced) set on {name}. Only one is allowed.");
    // }

    public EnumTransform Preset { get; set; }
    public string? Regex { get; set; }
    public string? CasePattern { get; set; }
    public EnumOrder SortMemberNames { get; set; }
    public EnumOrder SortMemberValues { get; set; }
    public EnumOrder SortUnderlyingValues { get; set; }
    public EnumOrder SortDisplayNames { get; set; }
    public EnumOrder SortDescriptions { get; set; }
}