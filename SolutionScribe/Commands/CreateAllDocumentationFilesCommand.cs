using SolutionScribe.Core.Services;
using SolutionScribe.Windows;

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

        // The license is the only file that asks the user anything, so it is settled before
        // anything is written and a cancel there abandons the whole command.
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

            foreach (var (fileName, getContent) in s_templateFiles)
            {
                if (solutionDirectory.Contains(fileName))
                {
                    report.Skipped(fileName);
                    continue;
                }

                solutionDirectory.Write(fileName, getContent());
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
