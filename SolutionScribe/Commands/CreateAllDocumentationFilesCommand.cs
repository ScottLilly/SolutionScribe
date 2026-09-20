using SolutionScribe.Core.Services;
using SolutionScribe.Windows;
using System.Collections.Generic;
using System.Text;

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

        var solutionFolder = await SolutionFolder.GetAsync();

        if (solutionFolder == null)
        {
            return;
        }

        var created = new List<string>();
        var skipped = new List<string>();

        // The license is the only file that asks the user anything, so it is settled before
        // anything is written and a cancel there abandons the whole command.
        string? licenseText = null;

        if (solutionFolder.Contains(LICENSE_FILE_NAME))
        {
            skipped.Add(LICENSE_FILE_NAME);
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
                solutionFolder.Write(LICENSE_FILE_NAME, licenseText);
                created.Add(LICENSE_FILE_NAME);
            }

            foreach (var (fileName, getContent) in s_templateFiles)
            {
                if (solutionFolder.Contains(fileName))
                {
                    skipped.Add(fileName);
                    continue;
                }

                solutionFolder.Write(fileName, getContent());
                created.Add(fileName);
            }
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create the files: {ex.Message}");
            return;
        }

        await VS.StatusBar.ShowMessageAsync(Summarize(created, skipped));
    }

    private static string Summarize(IList<string> created, IList<string> skipped)
    {
        var message = new StringBuilder("Solution Scribe ");

        message.Append(created.Count == 0
            ? "created nothing"
            : $"created {string.Join(", ", created)}");

        if (skipped.Count > 0)
        {
            message.Append($". Skipped {string.Join(", ", skipped)}, which already existed");
        }

        message.Append(".");

        return message.ToString();
    }
}
