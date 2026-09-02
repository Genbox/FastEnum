# FastEnum

[![NuGet](https://img.shields.io/nuget/v/Genbox.FastEnum.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Genbox.FastEnum/)
[![License](https://img.shields.io/github/license/Genbox/FastEnum)](https://github.com/Genbox/FastEnum/blob/master/LICENSE.txt)

### Description

A source generator to generate common methods for your enum types at compile-time. Print values, parse, or get the underlying value of enums without using reflection.

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

| API style       | Examples                                                    | Purpose                                 |
|-----------------|-------------------------------------------------------------|-----------------------------------------|
| Enum extensions | `GetString()`, `GetUnderlyingValue()`, `IsFlagSet()`        | Operate on a specific enum value.       |
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

Overrides the generated helper name and the default format-enum and extension-class names. This is useful when generated namespaces bring otherwise distinct enums into the same scope. For example, if your enum is named `MyEnum`, the generated helper can be accessed like this:

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

Here are benchmarks for calling different methods in .NET versus using FastEnum or [Enums.NET](https://github.com/TylerBrinkley/Enums.NET). Enums.NET is a high-performance library for working with enum values.

Results were produced with BenchmarkDotNet 0.15.8 on .NET 10.0.11 using an Intel Core i7-12700K. For measurements distinguishable from empty-method overhead, FastEnum is about 9-1,200x faster than the corresponding .NET or reflection APIs and 1.2-7.2x faster than Enums.NET. Measurements close to zero may be indistinguishable from the empty-method overhead.

| Method                        |        Mean |     Error |    StdDev |      Median |
|-------------------------------|------------:|----------:|----------:|------------:|
| EnumHasFlag                   |   0.0026 ns | 0.0050 ns | 0.0046 ns |   0.0000 ns |
| FastEnumHasFlag               |   0.0018 ns | 0.0047 ns | 0.0044 ns |   0.0000 ns |
| EnumsNetHasFlag               |   0.0055 ns | 0.0068 ns | 0.0063 ns |   0.0035 ns |
|                               |             |           |           |             |
| EnumIsDefined                 |  10.0648 ns | 0.0640 ns | 0.0568 ns |  10.0556 ns |
| FastEnumIsDefined             |   0.5623 ns | 0.0134 ns | 0.0126 ns |   0.5596 ns |
| EnumsNetIsDefined             |   0.0096 ns | 0.0061 ns | 0.0057 ns |   0.0102 ns |
| EnumIsDefinedFlags            |  10.4711 ns | 0.2234 ns | 0.2391 ns |  10.5022 ns |
| FastEnumIsDefinedFlags        |   0.0015 ns | 0.0033 ns | 0.0029 ns |   0.0000 ns |
| EnumsNetIsDefinedFlags        |   0.0087 ns | 0.0058 ns | 0.0052 ns |   0.0095 ns |
|                               |             |           |           |             |
| EnumLength                    |   8.9904 ns | 0.1671 ns | 0.1482 ns |   9.0109 ns |
| FastEnumLength                |   0.0056 ns | 0.0075 ns | 0.0070 ns |   0.0008 ns |
| EnumsNetLength                |   1.2076 ns | 0.0402 ns | 0.0376 ns |   1.2038 ns |
|                               |             |           |           |             |
| EnumGetNames                  |  11.3200 ns | 0.2221 ns | 0.2644 ns |  11.3051 ns |
| FastEnumGetNames              |   0.5888 ns | 0.0191 ns | 0.0169 ns |   0.5920 ns |
| EnumsNetGetNames              |   0.8262 ns | 0.0300 ns | 0.0281 ns |   0.8205 ns |
|                               |             |           |           |             |
| EnumToString                  |   6.3870 ns | 0.0772 ns | 0.0685 ns |   6.3940 ns |
| FastEnumToString              |   0.7065 ns | 0.0237 ns | 0.0222 ns |   0.7067 ns |
| EnumsNetToString              |   0.8633 ns | 0.0086 ns | 0.0071 ns |   0.8613 ns |
|                               |             |           |           |             |
| ReflectionGetDisplayName      | 534.1293 ns | 2.6270 ns | 2.4573 ns | 533.7293 ns |
| FastEnumGetDisplayName        |   0.4458 ns | 0.0086 ns | 0.0072 ns |   0.4429 ns |
| EnumsNetGetDisplayName        |   3.1976 ns | 0.0285 ns | 0.0267 ns |   3.1947 ns |
|                               |             |           |           |             |
| EnumTryParse                  |  12.2020 ns | 0.1346 ns | 0.1259 ns |  12.1766 ns |
| FastEnumTryParse              |   0.0055 ns | 0.0033 ns | 0.0031 ns |   0.0054 ns |
| EnumsNetTryParse              |   5.1037 ns | 0.0471 ns | 0.0441 ns |   5.0905 ns |
|                               |             |           |           |             |
| ReflectionTryParseDisplayName | 754.9752 ns | 5.4228 ns | 5.0725 ns | 753.8582 ns |
| FastEnumTryParseDisplayName   |   0.0017 ns | 0.0023 ns | 0.0022 ns |   0.0002 ns |
| EnumsNetTryParseDisplayName   |   7.7411 ns | 0.0459 ns | 0.0407 ns |   7.7366 ns |
|                               |             |           |           |             |
| EnumGetValues                 |   0.0018 ns | 0.0041 ns | 0.0034 ns |   0.0000 ns |
| FastEnumGetValues             |   0.0254 ns | 0.0169 ns | 0.0158 ns |   0.0241 ns |
| EnumsNetGetValues             |   0.0025 ns | 0.0037 ns | 0.0034 ns |   0.0000 ns |
|                               |             |           |           |             |
| EnumGetValues                 |  17.7414 ns | 0.1586 ns | 0.1484 ns |  17.7418 ns |
| FastEnumGetValues             |   0.6532 ns | 0.0205 ns | 0.0182 ns |   0.6536 ns |
| EnumsNetGetValues             |   0.8308 ns | 0.0113 ns | 0.0089 ns |   0.8327 ns |