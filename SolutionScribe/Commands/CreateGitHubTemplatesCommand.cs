using SolutionScribe.Core.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateGitHubTemplatesCommand)]
internal sealed class CreateGitHubTemplatesCommand :
    BaseCommand<CreateGitHubTemplatesCommand>
{
    // GitHub reads these from .github\ in the repository root, which for these solutions is the
    // solution directory. The names are GitHub's, not ours, and cannot be changed.
    private static readonly (string RelativePath, Func<string> GetContent)[] s_templates =
    [
        (@".github\ISSUE_TEMPLATE\bug_report.md", TemplateFileRepository.GetBugReportTemplate),
        (@".github\ISSUE_TEMPLATE\feature_request.md", TemplateFileRepository.GetFeatureRequestTemplate),
        (@".github\PULL_REQUEST_TEMPLATE.md", TemplateFileRepository.GetPullRequestTemplate)
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

        try
        {
            foreach (var (relativePath, getContent) in s_templates)
            {
                if (solutionDirectory.Contains(relativePath))
                {
                    report.Skipped(relativePath);
                    continue;
                }

                solutionDirectory.Write(relativePath, getContent());
                report.Wrote(relativePath);
            }
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create the GitHub templates: {ex.Message}");
            return;
        }

        await VS.StatusBar.ShowMessageAsync(report.ToString());
    }
}
