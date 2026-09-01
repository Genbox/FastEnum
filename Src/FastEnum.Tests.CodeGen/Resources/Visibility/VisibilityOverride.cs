// Visibility overrides should not exceed the enum visibility

[FastEnum(ExtensionClassVisibility = Visibility.Internal, EnumsClassVisibility = Visibility.Internal)]
public enum MyPublicEnum
{
    First
}