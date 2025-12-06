# Repository Guidelines

## Project Structure & Module Organization
- `Src/FastEnum`: Source generator and core extensions under `Genbox.FastEnum`.
- `Src/FastEnum.Tests.CodeGen`: Verifies generated sources and diagnostics against sample enums in `Code/` and `Resources/`.
- `Src/FastEnum.Tests.Functionality`: Runtime-facing tests of the generated APIs.
- `Src/FastEnum.Examples`: Minimal app showing attribute usage.
- `Src/FastEnum.Benchmarks`: BenchmarkDotNet harness for performance tracking.
- `Scripts/Build.ps1` and `Scripts/Publish.ps1`: Helper scripts for CI-style builds and packing.

## Build, Test, and Development Commands
- Restore: `dotnet restore FastEnum.sln`.
- Build: `dotnet build FastEnum.sln`.
- Tests: `dotnet test Src/FastEnum.Tests.CodeGen/FastEnum.Tests.CodeGen.csproj` and `dotnet test Src/FastEnum.Tests.Functionality/FastEnum.Tests.Functionality.csproj`.
- Examples: `dotnet run --project Src/FastEnum.Examples`.
- Benchmarks: `dotnet run -c Release --project Src/FastEnum.Benchmarks` (runs BenchmarkDotNet; expect warmup/measurement time).

## Coding Style & Naming Conventions
- C# `LangVersion` set to `latest`, `nullable` enabled, checked arithmetic in Debug; prefer modern BCL APIs and span-friendly code.
- Indent with four spaces; braces on new lines for types/methods, inline for statements when readable.
- Namespace root is `Genbox.*` via shared props; keep public surface in PascalCase, private fields camelCase with contextual names.
- Analyzer set includes Meziantou, Roslynator, Sonar, etc.; they run live in IDE (not during build). Treat warnings as actionable and avoid suppressions unless justified.
- Generated files use deterministic names (`_EnumFormat.g.cs`, `_Enums.g.cs`, `_Extensions.g.cs`); keep user-authored helpers in separate partials to avoid merge noise.

## Testing Guidelines
- xUnit v3 is used; keep test files named `*Tests.cs` and group scenarios by feature (codegen vs runtime).
- For generator changes, add/adjust sample enums under the relevant `Code/` folder and assert produced output/diagnostics; include flag enums and custom transforms when applicable.
- For runtime changes, cover parsing, formatting, and flag handling with positive/negative cases.