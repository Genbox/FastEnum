// Sorting options on EnumTransform (names, values, underlying, display, description)

[FastEnum]
[EnumTransform(SortMemberNames = EnumOrder.Ascending, SortMemberValues = EnumOrder.Descending, SortUnderlyingValues = EnumOrder.Ascending, SortDisplayNames = EnumOrder.Descending, SortDescriptions = EnumOrder.Ascending)]
public enum SortedEnum
{
    [Display(Name = "Bravo", Description = "Second")]
    One = 2,

    [Display(Name = "Alpha", Description = "First")]
    Two = 1,

    [Display(Name = "Charlie", Description = "Third")]
    Three = 3,
}