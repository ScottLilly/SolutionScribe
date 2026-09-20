# Architecture

What exists and why it is built this way. Work that is not built yet lives in
[GitHub Issues](https://github.com/ScottLilly/SolutionScribe/issues).

## Shape

```
SolutionScribe.sln
  SolutionScribe/            net48            VSIX, extension package, commands, dialogs,
                                              options page
  SolutionScribe.Core/       netstandard2.0   Licenses, templates, and what fills them in
  Tests.SolutionScribe.Core/ net48            MSTest coverage of SolutionScribe.Core, and of
                                              the repository files nothing else checks
```

## The Core boundary

`SolutionScribe.Core` must not reference `Microsoft.VisualStudio.*`, `EnvDTE`, or
`Community.VisualStudio.Toolkit`. That is what makes it testable without standing up Visual
Studio, and everything worth testing is on that side of the line: the license list and its
embedded texts, the document templates, placeholder substitution, and reading a GitHub remote out
of a `.git\config`.

What stays in the VSIX is the part that cannot run outside Visual Studio anyway: the package, the
commands, the two WPF dialogs and the options page. It is untested.

Both dialogs derive from `Microsoft.VisualStudio.PlatformUI.DialogWindow`, which supplies Visual
Studio's themed dialog styles and its `ShowModal`, so they follow the IDE's theme, font and DPI,
and are parented to the main window without a helper of their own. Their XAML sets no font, color
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

## Templates and their placeholders

The files in `SolutionScribe.Core/Templates` are embedded resources, written to disk with their
placeholders filled in from what the project details dialog asked for.

Each one is a default the user can override. `TemplateFileRepository` is constructed with a
template folder, taken from Tools > Options, and a file of the same relative path there wins over
the embedded resource. `TemplatePaths` is the list of those relative paths, and the layout matches
the embedded resources, so `GitHub\bug_report.md` is both where the user's copy goes and what names
the resource. A user file that exists and can be read is used as it stands, empty or not; anything
else, including a folder that does not exist or a locked file, falls back to the default rather
than failing the command. Add a template and it needs a row in `TemplatePaths`, or nobody can
override it and the export command will not write it out.

A placeholder is `<github user>`, `<repository>`, `<nuget package>` or `<security email>`, in the
same angle bracket form the license texts use for `<year>` and `<copyright holder>`. Square bracket
text such as `[Say how to install or run this project.]` is an instruction to whoever edits the
file afterwards, and is left alone on purpose.

Lines between `<!--#if nuget-->` or `<!--#if app-->` and `<!--#endif-->` are kept or dropped
according to whether a NuGet package name was given, and the markers come out either way. That is
what lets one README template serve both shapes instead of two templates drifting apart. Blocks do
not nest, and a condition the extension does not recognize keeps its lines, because silently
deleting part of someone's template is the worse of the two mistakes.

`ProjectDetails.HasPlaceholders` is what decides whether a command asks anything, so a template
with nothing to fill in, such as CHANGELOG.md, never shows the dialog. Add a placeholder to a
template and the prompt follows on its own.

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

`.github/workflows/build-and-test.yml` does the same two things on a clean `windows-latest`
runner and keeps the `.vsix` as an artifact. It runs only when someone starts it, from the Actions
tab or with `gh workflow run build-and-test.yml`. Nobody pushes here but the repository's owner,
who has just built locally, so a run per push would mostly re-prove what is already known. It is
worth starting before tagging a release, after a change to the csproj or the manifest, and any
time a build behaves differently on a clean machine than it does here.

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

## Decided against

Ideas that were considered and rejected, so the same one does not come back around in three months.
The reasoning travels with the entry: an item reduced to "decided against" gets re-proposed. Where
an entry concerns one member, it goes on that member's doc comment instead, because that is what
somebody reads before changing it.

- Nothing yet.

## Generated files that no command line build regenerates

`source.extension.cs` and `VSCommandTable.cs` are written by VSIX Synchronizer, and the VSCT
compiler reads `VSCommandTable.vsct` rather than the C#. Both generators are Visual Studio
extensions, so `MSBuild.exe` will happily build a stale or hand-edited copy of either file and say
nothing.

Editing them by hand is therefore sometimes necessary and always temporary: open the solution in
Visual Studio afterwards and let the tool rewrite them. `VsixManifestTests` fails when
`source.extension.cs` and the manifest disagree on the version, the description or the tags, which
is the half of this worth catching automatically. Nothing checks the command ids.

