# Architecture

What exists and why it is built this way. Work that is not built yet lives in
[BACKLOG.md](BACKLOG.md).

## Shape

```
SolutionScribe.sln
  SolutionScribe/            net48            VSIX, extension package, commands, license dialog,
                                              options page
  SolutionScribe.Core/       netstandard2.0   Licenses and templates
  Tests.SolutionScribe.Core/ net48            MSTest coverage of SolutionScribe.Core, and of
                                              the repository files nothing else checks
```

## The Core boundary

`SolutionScribe.Core` must not reference `Microsoft.VisualStudio.*`, `EnvDTE`, or
`Community.VisualStudio.Toolkit`. That is what makes it testable without standing up Visual
Studio, and everything worth testing is on that side of the line: the license list and its
embedded texts, the document templates, and placeholder substitution.

What stays in the VSIX is the part that cannot run outside Visual Studio anyway: the package, the
commands, the WPF dialog and the options page. It is untested.

The dialog derives from `Microsoft.VisualStudio.PlatformUI.DialogWindow`, which supplies Visual
Studio's themed dialog styles and its `ShowModal`, so the dialog follows the IDE's theme, font and
DPI, and is parented to the main window without a helper of its own. Its XAML sets no font, color
or pixel position; anything it does set comes from a `VsBrushes` key.

One consequence of the boundary: `netstandard2.0` is the one target the net48 VSIX and a test
project can both consume.

## Settings

`Options/GeneralOptions.cs` derives from the Community toolkit's `BaseOptionModel<T>`, which reads
and writes the Visual Studio settings store. `ProvideOptionPage` on the package registers it as
Tools > Options > Solution Scribe > General, with `SupportsProfiles` so the values travel through
Import and Export Settings.

That puts the settings where Visual Studio keeps everything else, rather than in a file under
AppData that nothing surfaces. The store is per Visual Studio installation, so a copyright holder
set in 2022 is not seen by 2026.

## Dependencies

`SolutionScribe.Core` has no package references. An extension shares a process with Visual Studio,
so a package whose version disagrees with the one VS loads fails at runtime on a machine nobody
can attach a debugger to, rather than at build time here. Keep it that way: everything Core needs
so far has been a few lines of its own code.

## Building and testing

`dotnet build` cannot build the VSIX project: `Microsoft.VSSDK.BuildTools` uses MSBuild tasks that
only exist on .NET Framework MSBuild. Build the solution with Visual Studio, or with
`MSBuild.exe -restore` from a Visual Studio install.

`dotnet test Tests.SolutionScribe.Core` works on its own, because neither it nor
`SolutionScribe.Core` touches the VSSDK.

The test project targets net48 rather than the net10.0 used elsewhere, because Visual Studio 2022
ships a .NET 9 SDK and cannot build a net10.0 project. Visual Studio 2026 ships a .NET 10 SDK, so
it can move whenever the solution no longer has to open in 2022.

## Versioning

`source.extension.vsixmanifest` holds the version, as the `Version` attribute on `<Identity>`, in
three parts so it matches the `Version x.y.z` milestone and release it ships as. Bump it when a
milestone opens, not at release time, so a build taken off the branch mid-milestone is not stamped
with the last release's number.

Nothing else in the repository states a version. `Vsix.Version` in `source.extension.cs` and the
assembly attributes in `Properties/AssemblyInfo.cs` both derive from the manifest.

## Generated files that no command line build regenerates

`source.extension.cs` and `VSCommandTable.cs` are written by VSIX Synchronizer, and the VSCT
compiler reads `VSCommandTable.vsct` rather than the C#. Both generators are Visual Studio
extensions, so `MSBuild.exe` will happily build a stale or hand-edited copy of either file and say
nothing.

Editing them by hand is therefore sometimes necessary and always temporary: open the solution in
Visual Studio afterwards and let the tool rewrite them. `VsixManifestTests` fails when
`source.extension.cs` and the manifest disagree on the version, the description or the tags, which
is the half of this worth catching automatically. Nothing checks the command ids.

