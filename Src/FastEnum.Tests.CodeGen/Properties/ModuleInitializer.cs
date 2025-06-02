using System.Runtime.CompilerServices;
using VerifyTests.DiffPlex;

namespace Genbox.FastEnum.Tests.CodeGen.Properties;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() => VerifyDiffPlex.Initialize(OutputType.Compact);
}