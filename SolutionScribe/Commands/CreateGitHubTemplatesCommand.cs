using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using SolutionScribe.Windows;
using System.Collections.Generic;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateGitHubTemplatesCommand)]
internal sealed class CreateGitHubTemplatesCommand :
    BaseCommand<CreateGitHubTemplatesCommand>
{
    // GitHub reads these from .github\ in the repository root, which for these solutions is the
    // solution directory. The names are GitHub's, not ours, and cannot be changed.
    private static readonly (string RelativePath, Func<TemplateFileRepository, string> GetContent)[] s_templates =
    [
        (@".github\ISSUE_TEMPLATE\bug_report.md", templates => templates.GetBugReportTemplate()),
        (@".github\ISSUE_TEMPLATE\feature_request.md", templates => templates.GetFeatureRequestTemplate()),
        (@".github\PULL_REQUEST_TEMPLATE.md", templates => templates.GetPullRequestTemplate())
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
        var pending = new List<(string RelativePath, string Template)>();
        var templates = TemplateSource.Current;

        foreach (var (relativePath, getContent) in s_templates)
        {
            if (solutionDirectory.Contains(relativePath))
            {
                report.Skipped(relativePath);
                continue;
            }

            pending.Add((relativePath, getContent(templates)));
        }

        ProjectDetails? projectDetails = null;

        if (pending.Exists(template => ProjectDetails.HasPlaceholders(template.Template)))
        {
            projectDetails = ProjectDetailsWindow.AskForProjectDetails(solutionDirectory);

            if (projectDetails == null)
            {
                return;
            }
        }

        try
        {
            foreach (var (relativePath, template) in pending)
            {
                solutionDirectory.Write(relativePath, projectDetails?.PopulateText(template) ?? template);
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
