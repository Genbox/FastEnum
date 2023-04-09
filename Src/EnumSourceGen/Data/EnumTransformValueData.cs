namespace Genbox.EnumSourceGen.Data;

internal class EnumTransformValueData
{
    public void Validate(string name)
    {
        int allowed = NameOverride != null ? 1 : 0;
        allowed += TransformPreset != EnumTransform.None ? 1 : 0;
        allowed += Transform != null ? 1 : 0;

        if (allowed > 1)
            throw new InvalidOperationException($"Multiple transforms (NameOverride, Simple, Advanced) set on {name}. Only one is allowed.");
    }

    public string? NameOverride { get; set; }
    public EnumTransform TransformPreset { get; set; }
    public string? Transform { get; set; }
}