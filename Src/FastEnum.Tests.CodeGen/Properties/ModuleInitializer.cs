using System.Runtime.CompilerServices;
using DiffEngine;
using VerifyTests.DiffPlex;

namespace Genbox.FastEnum.Tests.CodeGen.Properties;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize(OutputType.Compact);
        DiffRunner.Disabled = true;
        VerifyTests.VerifierSettings.ScrubLinesContaining("global::System.CodeDom.Compiler.GeneratedCodeAttribute");
    }
}