# FastEnum

[![NuGet](https://img.shields.io/nuget/v/Genbox.FastEnum.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Genbox.FastEnum/)
[![License](https://img.shields.io/github/license/Genbox/FastEnum)](https://github.com/Genbox/FastEnum/blob/master/LICENSE.txt)

### Description

A source generator to generate common methods for your enum types at compile-time.
Print values, parse, or get the underlying value of enums without using reflection.

### Features

* Intuitive API with discoverability through IntelliSense. All enums can be accessed via the `Enums` class.
* High-performance
    * Zero allocations whenever possible.
    * `GetMemberNames()`, `GetMemberValues()` etc. are cached by default. Use `DisableCache` to disable it.
    * `MemberCount` and `IsFlagEnum` are constants, allowing the compiler to fold them.
* Support for names and descriptions from [DisplayAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute?view=net-7.0).
* Support for flag enums, including composite values, negative values, duplicate aliases, transformed names and per-member omissions.
* Support for preset, regex, case-pattern and per-member name transformations, plus independent metadata sorting.
* Support for fully or selectively skipping enum values with `[EnumOmitValue]`.
* Support for public and internal enums, including empty enums and enums in the global namespace or non-generic containing types.
* Support for every C# enum underlying type, explicit/negative values and duplicate values.
* Support for string and span parsing by name, value, display name or description with configurable `StringComparison`.
* Support for duplicate enum names in different namespaces and escaped C# identifiers.
* Options for controlling namespaces, class names, and other generated-code details. See the Options section below.

### Examples

Let's create a simple enum and add the `[FastEnum]` attribute to it.

```csharp
[FastEnum]
public enum Color
{
    Red,
    Green,
    Blue
}
```

#### Extensions

Extensions tell you something about an enum value. For example, `MyEnum.Value1.GetString()` is equivalent to `MyEnum.Value1.ToString()` from .NET, but does not need to discover the name at runtime.

The following extensions are auto-generated:

```csharp
Color color = Color.Red;

Console.WriteLine("String value: " + color.GetString());
Console.WriteLine("Underlying value: " + color.GetUnderlyingValue());
```

Output:

```
String value: Red
Underlying value: 0
```

#### Enums class

`Enums` is a class that contains metadata about the auto-generated enum.

```csharp
Console.WriteLine("Number of members: " + Enums.Color.MemberCount);
Console.WriteLine("Parse: " + Enums.Color.Parse("Red"));
Console.WriteLine("Is Green part of the enum: " + Enums.Color.IsDefined(Color.Green));

PrintArray("Member names:", Enums.Color.GetMemberNames());
PrintArray("Underlying values:", Enums.Color.GetUnderlyingValues());
```

`PrintArray` simply iterates an array and lists the values on separate lines.

Output:

```
Number of members: 3
Parse: Red
Is Green part of the enum: True
Member names:
- Red
- Green
- Blue
Underlying values:
- 0
- 1
- 2
```

### Generated API overview

| API style | Examples | Purpose |
|-----------|----------|---------|
| Enum extensions | `GetString()`, `GetUnderlyingValue()`, `IsFlagSet()` | Operate on a specific enum value. |
| Metadata helper | `Enums.Color.TryParse()`, `GetMemberNames()`, `IsDefined()` | Parse values and inspect the enum type. |

### Values via attributes

#### DisplayAttribute

If you add [DisplayAttribute](https://learn.microsoft.com/dotnet/api/system.componentmodel.dataannotations.displayattribute) to an enum member, the source generator generates display-name and description APIs:

```csharp
[FastEnum]
internal enum MyEnum
{
    [Display(Name = "Value1Name", Description = "Value1Description")]
    Value1 = 1,
    Value2 = 2
}
```

FastEnum generates `GetDisplayName()` and `GetDescription()` extensions for the enum.

```csharp
MyEnum e = MyEnum.Value1;
Console.WriteLine("Display name: " + e.GetDisplayName());
Console.WriteLine("Description: " + e.GetDescription());
```

Prefer `TryGetDisplayName()`, `TryGetDescription()`, and `TryGetUnderlyingValue()` when you want a boolean plus `out` pattern instead of exceptions. `Enums.MyEnum.GetDisplayNames()` and `GetDescriptions()` return all included metadata pairs.

Output:

```
Display name: Value1Name
Description: Value1Description
```

#### FlagsAttribute

For an enum with [FlagsAttribute](https://learn.microsoft.com/dotnet/api/system.flagsattribute), FastEnum adds `IsFlagSet()` and recognizes valid composite values in `IsDefined()`, `TryGetUnderlyingValue()`, and `GetUnderlyingValue()`.

```csharp
[Flags]
[FastEnum]
internal enum MyFlagsEnum
{
    None = 0,
    Value1 = 1,
    Value2 = 2,
    Value3 = 4
}
```

```csharp
MyFlagsEnum e = MyFlagsEnum.Value1 | MyFlagsEnum.Value3;
Console.WriteLine("Is Value2 set: " + e.IsFlagSet(MyFlagsEnum.Value2));
Console.WriteLine("Composite value: " + e.GetUnderlyingValue());
```

Output:

```
Is Value2 set: False
Composite value: 5
```

### Options

`[FastEnum]` has several options that control the generated code.

#### ExtensionClassName

The generated extension class is `partial`. Set this to the name of your own partial extension class to combine generated and user-authored methods. The default is `<EnumName>Extensions`.

#### ExtensionClassNamespace

Controls the namespace containing the extension class. The default is the enum's namespace.

#### ExtensionClassVisibility

Use this to override the visibility of the generated extension class. It defaults to the enum's own visibility (`Visibility.Inherit`).

```csharp
[FastEnum(ExtensionClassVisibility = Visibility.Internal)] // Generates an internal StatusExtensions class instead of public.
public enum Status { Ok, Error }
```

#### EnumsClassName

Changes the name of the outer `Enums` wrapper.

#### EnumsClassNamespace

Controls the namespace containing the generated metadata helper and format enum. The default is the enum's namespace.

#### EnumsClassVisibility

Use this to override the visibility of the generated `Enums` wrapper class. It defaults to the enum's own visibility (`Visibility.Inherit`).

```csharp
[FastEnum(EnumsClassVisibility = Visibility.Internal)] // Enums.Status will be internal.
public enum Status { Ok, Error }
```

#### EnumNameOverride

Overrides the generated helper name and the default format-enum and extension-class names. This is useful when generated namespaces bring otherwise distinct enums into the same scope.
For example, if your enum is named `MyEnum`, the generated helper can be accessed like this:

```csharp
Enums.MyEnum.GetMemberNames()
```

If you set `EnumNameOverride` to `OtherEnum`, it will look like this instead:

```csharp
Enums.OtherEnum.GetMemberNames()
```

#### DisableEnumsWrapper

Removes the outer static `Enums` wrapper, changing `Enums.MyEnum` to `MyEnum`. Use `EnumNameOverride` or a different `EnumsClassNamespace` if the helper would otherwise collide with the enum type.

#### DisableCache

By default, arrays returned by metadata methods are cached to avoid repeat allocations. Set this option to return a new array on every call instead.

### Transformations

You can transform the string output of enums with `[EnumTransform]` at compile time. There are a few ways to do this.

```csharp
[EnumTransform(Preset = EnumTransform.UpperCase)] // Uppercase all enum values
[EnumTransform(Regex = "/^Enum//")] // Replace a leading "Enum" with nothing
[EnumTransform(CasePattern = "U_U_U")] // Uppercase the first, third, and fifth characters
```

You can specify only one `[EnumTransform]` per enum.

*Regex* must have the format `/regex-here/replacement-here/`.

*CasePattern* can uppercase, lowercase, or omit characters.

The language uses the following modifier characters:

* U: Uppercase the character.
* L: Lowercase the character.
* O: Omit the character.
* _: Keep the character as-is.

Let's say you want to omit the first character in all values, uppercase the third character and lowercase the rest.

```csharp
[FastEnum]
[EnumTransform(CasePattern = "OOULLLLL")]
public enum MyEnum
{
    Myvalue1,
    Myvalue2,
    Myvalue3
}
```

The pattern is matched as much as possible. A pattern of `U` will simply uppercase the first character, and a pattern of `UUUUUUUUUUUU` will uppercase the first 12 characters, even if the enum value is only 6 characters long.

`[EnumTransform]` options:

* `Preset` uppercases or lowercases all member names.
* `Regex` allows replacing a pattern.
* `CasePattern` applies a simple U/L/O/_ mask.
* `SortMemberNames`, `SortMemberValues`, `SortUnderlyingValues`, `SortDisplayNames`, and `SortDescriptions` control the corresponding generated arrays. Each accepts `EnumOrder.None` (declaration order), `Ascending`, or `Descending`.

```csharp
[FastEnum]
[EnumTransform(Preset = EnumTransform.UpperCase)]
public enum Color { Red, Green }
// GetString(Color.Red) => "RED"

[FastEnum]
[EnumTransform(Regex = "/^Clr//")]
public enum Color { ClrRed, ClrGreen }
// GetString(Color.ClrRed) => "Red"

[FastEnum]
[EnumTransform(CasePattern = "U____")]
public enum Color { apple, pears }
// GetString(Color.apple) => "Apple"
// GetString(Color.pears) => "Pears"

[FastEnum]
[EnumTransform(SortMemberNames = EnumOrder.Descending)]
public enum Nato { Alpha, Bravo, Charlie }
// GetMemberNames() => ["Charlie", "Bravo", "Alpha"]
```

You can override the string for specific members with `[EnumTransformValue(ValueOverride = "...")]`. This is useful when most values follow a pattern but a few need custom text.

`[EnumTransformValue]` options:

* `ValueOverride` changes the generated string for that member and what `TryParse` will accept for it.

```csharp
[FastEnum]
public enum Status
{
    [EnumTransformValue(ValueOverride = "all good")]
    Ok,
    Error
}
// GetString(Status.Ok) => "all good"
// Enums.Status.TryParse("all good", out var s) => true
```

### Omitting values

Enum members can be omitted from all generated APIs or from selected APIs. This is useful when an enum populates a UI list but some values should not be shown.

```csharp
[FastEnum]
public enum Color
{
    [EnumOmitValue] // Completely omitted
    Unknown,

    [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberNames)] // Partially omitted
    Red,
    Green
}
```

If you call `GetMemberNames()` or any other method on the `Enums.Color` class, the Unknown value will be omitted.

```csharp
foreach (string name in Enums.Color.GetMemberNames())
{
    Console.WriteLine(name);
}
```

Output:

```
Green
```

`[EnumOmitValue]` options:

* `Exclude` is a flag enum controlling which generated APIs omit the member. Defaults to `EnumOmitExclude.All` when not specified.

```csharp
[FastEnum]
public enum Color
{
    [EnumOmitValue] // Omitted everywhere
    Unknown,

    [EnumOmitValue(Exclude = EnumOmitExclude.GetMemberNames | EnumOmitExclude.TryParse)]
    Red, // Shown in values but hidden from names and parsing
    Green
}

// Enums.Color.GetMemberNames() => ["Green"]
// Enums.Color.TryParse("Red", out _) => false
// Enums.Color.GetMemberValues() => [Color.Red, Color.Green]
```

Only `GetMemberNames()` and `TryParse()` exclude `Red`, so it remains available through `GetMemberValues()`.

```csharp
foreach (Color value in Enums.Color.GetMemberValues())
{
    Console.WriteLine(value.ToString());
}
```

Output:

```
Red
Green
```

### Limitations

* Enums must be `public` or `internal`; private and protected nested enums are not supported, and containing types cannot be less visible than the enum.
* Enums inside generic containing types are not supported.
* An enum can have only one `[EnumTransform]` attribute.

### Notes

#### Parse/TryParse methods

FastEnum has some additional features compared to .NET's `Enum.Parse<T>()` and `Enum.TryParse<T>()`:

* Supports [StringComparison](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparison?view=net-7.0), defaulting to ordinal comparison.
* Supports parsing `ValueOverride` when using `[EnumTransformValue]`, plus `DisplayName` and `Description` from [DisplayAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute?view=net-7.0).
* Allows `Name`, `Value`, `DisplayName`, and `Description` parsing to be selected with a format enum: `Enums.MyEnum.TryParse("val", out MyEnum v, MyEnumFormat.Name | MyEnumFormat.DisplayName)`.
* Overloads accept both `string` and `ReadOnlySpan<char>` to avoid unnecessary allocations when parsing substrings.

#### IsDefined method

The `IsDefined` method differs from the one provided by .NET and supports flags. `Enums.MyEnum.IsDefined((MyEnum)42)`
and `Enums.MyEnum.IsDefined(MyEnum.Value1 | MyEnum.Value3)` both work.

### Benchmarks

Here are benchmarks for calling different methods in .NET versus using CodeGen or [Enums.NET](https://github.com/TylerBrinkley/Enums.NET).
Enums.NET is a high-performance library for working with enum values.

The table below shows that Enums.NET is between 2-80x faster than .NET, but FastEnum is 2-14x faster than Enums.NET.

| Method                        |          Mean |      Error |     StdDev |        Median |
|-------------------------------|--------------:|-----------:|-----------:|--------------:|
| EnumHasFlag                   |     0.0056 ns |  0.0083 ns |  0.0074 ns |     0.0030 ns |
| CodeGenHasFlag                |     0.0101 ns |  0.0062 ns |  0.0058 ns |     0.0083 ns |
| EnumsNetHasFlag               |     0.6253 ns |  0.0238 ns |  0.0211 ns |     0.6192 ns |
|                               |               |            |            |               |
| EnumIsDefined                 |    42.6278 ns |  0.4633 ns |  0.4334 ns |    42.5181 ns |
| CodeGenIsDefined              |     0.0045 ns |  0.0066 ns |  0.0058 ns |     0.0013 ns |
| EnumsNetIsDefined             |     0.4842 ns |  0.0101 ns |  0.0085 ns |     0.4845 ns |
|                               |               |            |            |               |
| EnumLength                    |    20.5705 ns |  0.3509 ns |  0.5566 ns |    20.5117 ns |
| CodeGenLength                 |     0.0001 ns |  0.0004 ns |  0.0003 ns |     0.0000 ns |
| EnumsNetLength                |     5.3362 ns |  0.1085 ns |  0.0906 ns |     5.3495 ns |
|                               |               |            |            |               |
| EnumGetNames                  |    17.5890 ns |  0.2905 ns |  0.2717 ns |    17.6505 ns |
| CodeGenGetNames               |     1.5764 ns |  0.0476 ns |  0.0446 ns |     1.5843 ns |
| EnumsNetGetNames              |     1.6052 ns |  0.0323 ns |  0.0286 ns |     1.5994 ns |
|                               |               |            |            |               |
| EnumToString                  |    15.3389 ns |  0.1815 ns |  0.1698 ns |    15.3113 ns |
| CodeGenToString               |     0.8379 ns |  0.0380 ns |  0.0337 ns |     0.8267 ns |
| EnumsNetToString              |    21.3269 ns |  0.0836 ns |  0.0653 ns |    21.3056 ns |
|                               |               |            |            |               |
| ReflectionGetDisplayName      | 1,008.3327 ns |  6.6111 ns |  5.8606 ns | 1,007.1464 ns |
| CodeGenGetDisplayName         |     3.2780 ns |  0.1182 ns |  0.1495 ns |     3.2501 ns |
| EnumsNetGetDisplayName        |     9.3953 ns |  0.1237 ns |  0.0966 ns |     9.4036 ns |
|                               |               |            |            |               |
| EnumTryParse                  |    31.4662 ns |  0.4013 ns |  0.3557 ns |    31.3008 ns |
| CodeGenTryParse               |     6.4197 ns |  0.0495 ns |  0.0463 ns |     6.4246 ns |
| EnumsNetTryParse              |    41.8299 ns |  0.4277 ns |  0.4001 ns |    41.7159 ns |
|                               |               |            |            |               |
| ReflectionTryParseDisplayName | 1,763.8422 ns | 17.7619 ns | 15.7455 ns | 1,764.9430 ns |
| CodeGenTryParseDisplayName    |     3.9371 ns |  0.0177 ns |  0.0138 ns |     3.9381 ns |
| EnumsNetTryParseDisplayName   |    40.4373 ns |  0.7966 ns |  0.7061 ns |    40.6816 ns |
|                               |               |            |            |               |
| EnumGetValues                 |     0.0000 ns |  0.0000 ns |  0.0000 ns |     0.0000 ns |
| CodeGenGetValues              |     2.2281 ns |  0.0281 ns |  0.0263 ns |     2.2237 ns |
| EnumsNetGetValues             |     4.6160 ns |  0.0868 ns |  0.0769 ns |     4.6156 ns |
|                               |               |            |            |               |
| EnumGetValues                 |   254.3910 ns |  2.2232 ns |  1.8565 ns |   253.8503 ns |
| CodeGenGetValues              |     1.6647 ns |  0.0508 ns |  0.0475 ns |     1.6750 ns |
| EnumsNetGetValues             |     2.4889 ns |  0.0416 ns |  0.0389 ns |     2.4874 ns |