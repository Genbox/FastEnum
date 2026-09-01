// EnumTransformValue override should affect GetString/TryParse

[FastEnum]
public enum Status
{
    [EnumTransformValue(ValueOverride = "good")]
    Ok,
    Error
}