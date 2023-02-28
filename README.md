# EnumSourceGen

[![NuGet](https://img.shields.io/nuget/v/Genbox.EnumSourceGen.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Genbox.EnumSourceGen/)
[![License](https://img.shields.io/github/license/Genbox/EnumSourceGen)](https://github.com/Genbox/EnumSourceGen/blob/master/LICENSE.txt)

### Description

A source generator to generate common methods for your enum types at compile-time.
Print values, parse, or get the underlying value of enums without using reflection.

### Features

* Intuitive API with discoverability through IntelliSense. All enums can be accessed via the `Enums` class.
* High-performance
    * Zero allocations whenever possible.
    * `MemberCount` and `IsFlagsEnum` is const to allow the compiler to fold constants.
* Supports name and description from [DisplayAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute?view=net-7.0).
    * Only generates `GetDisplayName()` and `GetDescription()`extensions when the attribute is present.
* Support for flag enums via the [FlagsAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.flagsattribute?view=net-7.0).
    * Only generates `IsFlagSet()` when the attribute is present.
* Support for private/internal enums
* Support for enums nested inside classes
* Support for user-set underlying values such as long, uint, byte, etc.
* Support for duplicate enum names from different namespaces
* Support for enums that reside in the global namespace
* Has several options to control namespace, class name and more for generated code. See Options section below for details.

### Parse/TryParse methods
EnumSourceGen has some additional features compared to dotnet's `Enum.Parse<T>()` and `Enum.TryParse<T>()`:
* Supports [StringComparison](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparison?view=net-7.0) (defaults to ordinal comparison)
* Supports parsing `DisplayName` and `Description` from [DisplayAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.displayattribute?view=net-7.0)
* You can enable/disable Name, Value, DisplayName or Description parsing via a flags enum: `Enums.MyEnum.TryParse("val", out MyEnum v, MyEnumFormat.Name | MyEnumFormat.DisplayName)`

### IsDefined method
The IsDefined method is different than the one provided by dotnet. It supports flags out of the box. `Enums.MyEnum.IsDefined((MyEnum)42)` and `Enums.MyEnum.IsDefined(MyEnum.Value1 | MyEnum.Value3)` both work.

### Examples

For the code sections below, the following enum is defined:

```csharp
[EnumSourceGen]
[Flags]
internal enum MyEnum
{
    [Display(Name = "Value1Name", Description = "Value1Description")]
    Value1 = 1,
    [Display(Name = "DisplayNameForValue2")]
    Value2 = 2,
    [Display(Description = "Description for Value3")]
    Value3 = 4
}
```

#### Extensions

The following extensions are auto-generated on MyEnum:

```csharp
MyEnum e = MyEnum.Value1;

Console.WriteLine("String value: " + e.GetString());
Console.WriteLine("Underlying value: " + e.GetUnderlyingValue());
Console.WriteLine("Display name: " + e.GetDisplayName());
Console.WriteLine("Description: " + e.GetDescription());
Console.WriteLine("Has Value1 flag: " + e.IsFlagSet(MyEnum.Value1));
```

Output from the code above:

```
String value: Value1
Underlying value: 1
Display name: Value1Name
Description: Value1Description
Has Value1 flag: True
```

#### Enums class

`Enums` is a class that contains functionality for the auto-generated enum.

```csharp
Console.WriteLine("Number of members: " + Enums.MyEnum.MemberCount);
Console.WriteLine("Parse: " + Enums.MyEnum.Parse("value1", StringComparison.OrdinalIgnoreCase));
Console.WriteLine("TryParse success: " + Enums.MyEnum.TryParse("value1", out MyEnum val, StringComparison.OrdinalIgnoreCase) + " value: " + val);
Console.WriteLine("Is Value1 part of the enum: " + Enums.MyEnum.IsDefined(MyEnum.Value1));

PrintArray("Member names:", Enums.MyEnum.GetMemberNames());
PrintArray("Member values:", Enums.MyEnum.GetMemberValues());
PrintArray("Underlying values:", Enums.MyEnum.GetUnderlyingValues());
PrintArray("Display names:", Enums.MyEnum.GetDisplayNames());
PrintArray("Descriptions:", Enums.MyEnum.GetDescriptions());
```

PrintArray simply iterates an array and list the values on separate lines.

Output from the code above:

```
Number of members: 3
Parse: Value1
TryParse success: True value: Value1
Is Value1 part of the enum: True
Member names:
- Value1
- Value2
- Value3
Member values:
- Value1
- Value2
- Value3
Underlying values:
- 1
- 2
- 4
Display names:
- (Value1, Value1Name)
- (Value2, DisplayNameForValue2)
Descriptions:
- (Value1, Value1Description)
- (Value3, Description for Value3)
```

### Options

`EnumSourceGenAttribute` have several options to control the behavior of the generated code. They are specified in the constructor.

```csharp
[EnumSourceGen(Generate = Generate.ClassAndExtensions, ExtensionClassName = "MyEnumExtensions")]
internal enum MyEnum
{
    Value,
}
```

##### Generate

You can set 3 different values for this option:

* ClassAndExtensions (default): Generates both a static Enums class member and extensions
* ClassOnly: Only generate the static Enum class member
* ExtensionsOnly: Only generate the enum extensions

It is best practice to only enable what you need to keep the size of your assemblies small.

##### ExtensionClassName

The generated extension class is `partial` by default. So if you want to combine extension from your own class and the autogenerated one, you can use this option to set the name to
the same as your extensions class. Defaults to &lt;EnumName&gt;Extensions.

##### ExtensionClassNamespace

Use this to control which namespace the extensions class belongs to. Defaults to the namespace of the enum.

##### EnumsClassName

Use this to set the name of the `Enums` class to something else.

##### EnumsClassNamespace

Used this to specify the namespace for the Enums class. Defaults to the namespace of the enum.

##### EnumNameOverride

Sometimes you might have two enums named the same, but in different namespaces. You can use this option to override the name of the enum in the generated code.
For example, if your enum is named `MyEnum` the Enums class can be accessed like this:
```csharp
Enums.MyEnum.GetMemberNames()
```

If you set EnumNameOverride to `OtherEnum` it will look like this instead:
```csharp
Enums.OtherEnum.GetMemberNames()
```

