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
* Support for public, internal, and accessible protected internal enums, including empty enums and enums in the global namespace or non-generic containing types.
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

`MemberCount` and `IsFlagEnum` describe the enum. `GetMemberNames()`, `GetMemberValues()`, and `GetUnderlyingValues()` return its included members.

`GetString(ColorFormat)` selects `Name`, invariant numeric `Value`, or available `DisplayName`/`Description` metadata. Formats can be combined; `Default` is `Name | Value`. Formatting prefers display name, description, name, then value, and falls back to `Enum.ToString()` when none matches. `None` uses that fallback directly.

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

Multiple enums can share an extension class when their effective extension-class visibility matches. Shared `Enums` wrappers become public if any generated helper requires it.

#### ExtensionClassNamespace

Controls the namespace containing the extension class. The default is the enum's namespace.

#### ExtensionClassVisibility

Use this to override the visibility of the generated extension class. It defaults to the enum's effective visibility, including its containing types (`Visibility.Inherit`).

```csharp
[FastEnum(ExtensionClassVisibility = Visibility.Internal)] // Generates an internal StatusExtensions class instead of public.
public enum Status { Ok, Error }
```

#### EnumsClassName

Changes the name of the outer `Enums` wrapper.

#### EnumsClassNamespace

Controls the namespace containing the generated metadata helper and format enum. The default is the enum's namespace.

#### EnumsClassVisibility

Use this to override the visibility of the generated `Enums` wrapper class. It defaults to the enum's effective visibility, including its containing types (`Visibility.Inherit`).

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

Targets are `GetMemberNames`, `GetMemberValues`, `GetUnderlyingValues`, `TryGetUnderlyingValue`, `TryParse`, `TryGetDisplayName`, `TryGetDescription`, `IsDefined`, and `GetString`; combine them with `|`, or use `All`/`None`.

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

| Method                        |        Mean |      Error |     StdDev |      Median |
|-------------------------------|------------:|-----------:|-----------:|------------:|
| EnumHasFlag                   |   0.0028 ns |  0.0019 ns |  0.0017 ns |   0.0028 ns |
| FastEnumHasFlag               |   0.0033 ns |  0.0028 ns |  0.0023 ns |   0.0038 ns |
| EnumsNetHasFlag               |   0.0015 ns |  0.0049 ns |  0.0044 ns |   0.0000 ns |
|                               |             |            |            |             |
| EnumIsDefined                 |  11.3809 ns |  0.2514 ns |  0.5249 ns |  11.2634 ns |
| FastEnumIsDefined             |   0.0009 ns |  0.0025 ns |  0.0020 ns |   0.0000 ns |
| EnumsNetIsDefined             |   0.1360 ns |  0.0188 ns |  0.0157 ns |   0.1362 ns |
| EnumIsDefinedFlags            |  10.4597 ns |  0.2145 ns |  0.3868 ns |  10.3931 ns |
| FastEnumIsDefinedFlags        |   0.0335 ns |  0.0194 ns |  0.0172 ns |   0.0371 ns |
| EnumsNetIsDefinedFlags        |   0.0265 ns |  0.0245 ns |  0.0318 ns |   0.0089 ns |
|                               |             |            |            |             |
| EnumLength                    |   9.1790 ns |  0.2088 ns |  0.4670 ns |   9.0306 ns |
| FastEnumLength                |   0.0086 ns |  0.0048 ns |  0.0040 ns |   0.0074 ns |
| EnumsNetLength                |   1.3818 ns |  0.0131 ns |  0.0116 ns |   1.3847 ns |
|                               |             |            |            |             |
| EnumGetNames                  |  11.4278 ns |  0.2603 ns |  0.3196 ns |  11.3700 ns |
| FastEnumGetNames              |   0.5697 ns |  0.0296 ns |  0.0262 ns |   0.5662 ns |
| EnumsNetGetNames              |   0.8771 ns |  0.0422 ns |  0.0374 ns |   0.8855 ns |
|                               |             |            |            |             |
| EnumToString                  |   6.2842 ns |  0.1541 ns |  0.2444 ns |   6.1861 ns |
| FastEnumToString              |   0.4599 ns |  0.0259 ns |  0.0242 ns |   0.4596 ns |
| EnumsNetToString              |   0.9274 ns |  0.0604 ns |  0.0993 ns |   0.9182 ns |
|                               |             |            |            |             |
| ReflectionGetDisplayName      | 508.2034 ns |  9.7851 ns | 24.7281 ns | 497.2392 ns |
| FastEnumGetDisplayName        |   0.4649 ns |  0.0207 ns |  0.0173 ns |   0.4602 ns |
| EnumsNetGetDisplayName        |   3.1151 ns |  0.0628 ns |  0.0524 ns |   3.1055 ns |
|                               |             |            |            |             |
| EnumTryParse                  |  11.2563 ns |  0.2212 ns |  0.1847 ns |  11.2343 ns |
| FastEnumTryParse              |   0.0018 ns |  0.0043 ns |  0.0040 ns |   0.0000 ns |
| EnumsNetTryParse              |   5.2431 ns |  0.1274 ns |  0.1516 ns |   5.2066 ns |
|                               |             |            |            |             |
| ReflectionTryParseDisplayName | 756.3817 ns | 14.5033 ns | 39.4573 ns | 741.5761 ns |
| FastEnumTryParseDisplayName   |   0.0017 ns |  0.0054 ns |  0.0045 ns |   0.0000 ns |
| EnumsNetTryParseDisplayName   |   7.9300 ns |  0.1603 ns |  0.2931 ns |   7.8997 ns |
|                               |             |            |            |             |
| EnumGetValues                 |   0.0278 ns |  0.0157 ns |  0.0139 ns |   0.0252 ns |
| FastEnumGetValues             |   0.0047 ns |  0.0100 ns |  0.0094 ns |   0.0000 ns |
| EnumsNetGetValues             |   0.0119 ns |  0.0183 ns |  0.0203 ns |   0.0000 ns |
|                               |             |            |            |             |
| EnumGetValues                 |  17.7787 ns |  0.1652 ns |  0.1380 ns |  17.7083 ns |
| FastEnumGetValues             |   0.6865 ns |  0.0561 ns |  0.1323 ns |   0.6460 ns |
| EnumsNetGetValues             |   0.6081 ns |  0.0390 ns |  0.0365 ns |   0.5991 ns |