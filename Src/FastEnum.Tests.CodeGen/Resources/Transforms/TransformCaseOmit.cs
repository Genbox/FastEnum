// CasePattern with omit (O) should drop characters in GetString/GetMemberNames.

[FastEnum]
[EnumTransform(CasePattern = "OUU")]
public enum CasePatternEnum
{
    One,
    Two
}