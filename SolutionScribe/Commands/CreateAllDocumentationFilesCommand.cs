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

    private static readonly (string FileName, Func<string> GetContent)[] s_templateFiles =
    [
        ("README.md", TemplateFileRepository.GetReadmeTemplate),
        ("CHANGELOG.md", TemplateFileRepository.GetChangelogTemplate),
        ("CONTRIBUTING.md", TemplateFileRepository.GetContributingTemplate),
        ("CODE_OF_CONDUCT.md", TemplateFileRepository.GetCodeOfConductTemplate),
        ("SECURITY.md", TemplateFileRepository.GetSecurityTemplate)
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

        foreach (var (fileName, getContent) in s_templateFiles)
        {
            if (solutionDirectory.Contains(fileName))
            {
                report.Skipped(fileName);
                continue;
            }

            pending.Add((fileName, getContent()));
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
