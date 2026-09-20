using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using SolutionScribe.Windows;
using System.Collections.Generic;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateAllDocumentationFilesCommand)]
internal sealed class CreateAllDocumentationFilesCommand :
    BaseCommand<CreateAllDocumentationFilesCommand>
{
    private const string LICENSE_FILE_NAME = "LICENSE.txt";

    private static readonly (string FileName, Func<TemplateFileRepository, string> GetContent)[] s_templateFiles =
    [
        ("README.md", templates => templates.GetReadmeTemplate()),
        ("CHANGELOG.md", templates => templates.GetChangelogTemplate()),
        ("CONTRIBUTING.md", templates => templates.GetContributingTemplate()),
        ("CODE_OF_CONDUCT.md", templates => templates.GetCodeOfConductTemplate()),
        ("SECURITY.md", templates => templates.GetSecurityTemplate())
    ];

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var solutionDirectory = await SolutionDirectory.GetAsync();

        if (solutionDirectory == null)
        {
            return;
        }

        var report = new CreatedFilesReport();

        // Everything the user is asked is settled before anything is written, so a cancel at
        // either dialog abandons the whole command rather than leaving half the files behind.
        var pending = new List<(string FileName, string Template)>();
        var templates = TemplateSource.Current;

        foreach (var (fileName, getContent) in s_templateFiles)
        {
            if (solutionDirectory.Contains(fileName))
            {
                report.Skipped(fileName);
                continue;
            }

            pending.Add((fileName, getContent(templates)));
        }

        ProjectDetails? projectDetails = null;

        if (pending.Exists(file => ProjectDetails.HasPlaceholders(file.Template)))
        {
            projectDetails = ProjectDetailsWindow.AskForProjectDetails(solutionDirectory);

            if (projectDetails == null)
            {
                return;
            }
        }

        string? licenseText = null;

        if (solutionDirectory.Contains(LICENSE_FILE_NAME))
        {
            report.Skipped(LICENSE_FILE_NAME);
        }
        else
        {
            licenseText = LicenseDataWindow.AskForLicenseText();

            if (licenseText == null)
            {
                return;
            }
        }

        try
        {
            if (licenseText != null)
            {
                solutionDirectory.Write(LICENSE_FILE_NAME, licenseText);
                report.Wrote(LICENSE_FILE_NAME);
            }

            foreach (var (fileName, template) in pending)
            {
                solutionDirectory.Write(fileName, projectDetails?.PopulateText(template) ?? template);
                report.Wrote(fileName);
            }
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create the files: {ex.Message}");
            return;
        }

        await VS.StatusBar.ShowMessageAsync(report.ToString());
    }
}
