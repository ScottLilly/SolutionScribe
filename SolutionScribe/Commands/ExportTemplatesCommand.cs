using SolutionScribe.Core.Services;
using System.Diagnostics;
using System.IO;

namespace SolutionScribe.Commands;

/// <summary>
/// Writes the built-in templates into the user's template folder, so there is something to edit
/// rather than a blank folder and a guess at the file names. Unlike every other command, this one
/// writes nothing into the solution and so does not need one open.
/// </summary>
[Command(PackageIds.ExportTemplatesCommand)]
internal sealed class ExportTemplatesCommand : BaseCommand<ExportTemplatesCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        string folder = TemplateSource.Current.UserTemplateFolder;

        if (folder.Length == 0)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe",
                "No template folder is set. Set one under Tools > Options > Solution Scribe.");

            return;
        }

        int written = 0;
        int skipped = 0;

        try
        {
            foreach (string relativePath in TemplateFileRepository.TemplatePaths)
            {
                string targetPath = Path.Combine(folder, relativePath);

                // An existing file is someone's edited template. Overwriting it would be the one
                // thing this command must never do.
                if (File.Exists(targetPath))
                {
                    skipped++;

                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.WriteAllText(targetPath, TemplateFileRepository.GetEmbeddedTemplate(relativePath));

                written++;
            }
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe",
                $"Failed to export the templates to {folder}: {ex.Message}");

            return;
        }

        OpenFolder(folder);

        await VS.StatusBar.ShowMessageAsync(skipped == 0
            ? $"Solution Scribe exported {written} templates to {folder}."
            : $"Solution Scribe exported {written} templates to {folder}, and left {skipped} alone that were already there.");
    }

    /// <summary>
    /// The point of exporting is to edit them, so land the user in the folder rather than leaving
    /// them to find it from a status bar message that is about to disappear.
    /// </summary>
    private static void OpenFolder(string folder)
    {
        try
        {
            Process.Start(folder);
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is FileNotFoundException)
        {
            // The files are written either way, and failing to open a window is not worth an error
            // dialog in front of a command that succeeded.
            ex.Log();
        }
    }
}
