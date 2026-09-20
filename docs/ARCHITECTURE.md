# Architecture

What exists and why it is built this way. Work that is not built yet lives in
[BACKLOG.md](BACKLOG.md).

## Shape

```
SolutionScribe.sln
  SolutionScribe/            net48            VSIX, extension package, commands, license dialog
  SolutionScribe.Core/       netstandard2.0   Licenses, templates, settings
  Tests.SolutionScribe.Core/ net48            MSTest coverage of SolutionScribe.Core
```

## The Core boundary

`SolutionScribe.Core` must not reference `Microsoft.VisualStudio.*`, `EnvDTE`, or
`Community.VisualStudio.Toolkit`. That is what makes it testable without standing up Visual
Studio, and everything worth testing is on that side of the line: the license list and its
embedded texts, the document templates, placeholder substitution, and the settings file.

What stays in the VSIX is the part that cannot run outside Visual Studio anyway: the package, the
commands, and the WinForms dialog. It is untested.

Two consequences of the boundary:

- `SettingsRepository` takes its file path and an error callback rather than reading a static
  AppData path and logging to the Visual Studio output pane itself. The dialog passes
  `SettingsRepository.DefaultSettingsFilePath` and `ex => ex.Log()`.
- `netstandard2.0` is the one target the net48 VSIX and a test project can both consume.

## Building and testing

`dotnet build` cannot build the VSIX project: `Microsoft.VSSDK.BuildTools` uses MSBuild tasks that
only exist on .NET Framework MSBuild. Build the solution with Visual Studio, or with
`MSBuild.exe -restore` from a Visual Studio install.

`dotnet test Tests.SolutionScribe.Core` works on its own, because neither it nor
`SolutionScribe.Core` touches the VSSDK.

The test project targets net48 rather than the net10.0 used elsewhere, because Visual Studio 2022
ships a .NET 9 SDK and cannot build a net10.0 project. It can move once the extension supports
Visual Studio 2026.

