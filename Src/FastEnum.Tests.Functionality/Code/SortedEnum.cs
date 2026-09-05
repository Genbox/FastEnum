using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Genbox.FastEnum.Tests.Functionality.Code;

[FastEnum]
[EnumTransform(SortMemberNames = EnumOrder.Ascending, SortMemberValues = EnumOrder.Descending, SortUnderlyingValues = EnumOrder.Ascending, SortDisplayNames = EnumOrder.Descending, SortDescriptions = EnumOrder.Ascending)]
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "The fixture uses compact non-zero values to make every ordering observable.")]
internal enum SortedEnum
{
    [Display(Name = "Bravo", Description = "Second")]
    One = 2,
    [Display(Name = "Alpha", Description = "First")]
    Two = 1,
    [Display(Name = "Charlie", Description = "Third")]
    Three = 3
}