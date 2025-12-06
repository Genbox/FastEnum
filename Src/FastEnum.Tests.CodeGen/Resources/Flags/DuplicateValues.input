// Duplicate values in flag enums should still parse correctly

namespace Some.Namespace.Here;

[Flags]
[FastEnum]
public enum Profile
{
    Unknown = 0,
    WindowsLegacy = 1 << 0,
    WindowsServer2022 = 1 << 8,
    NewestServer = WindowsServer2022,
}