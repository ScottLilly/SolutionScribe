using Microsoft.VisualStudio.PlatformUI;
using SolutionScribe.Commands;
using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using SolutionScribe.Options;
using System.Windows;

namespace SolutionScribe.Windows;

public partial class ProjectDetailsWindow : DialogWindow
{
    private ProjectDetails? _projectDetails;

    /// <summary>
    /// Asks for the details the templates need, or returns null if the user canceled. Must be
    /// called on the UI thread.
    /// </summary>
    internal static ProjectDetails? AskForProjectDetails(SolutionDirectory solutionDirectory)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dialog = new ProjectDetailsWindow(solutionDirectory);

        return dialog.ShowModal() == true ? dialog._projectDetails : null;
    }

    // Internal because SolutionDirectory is, and nothing outside the extension creates this.
    internal ProjectDetailsWindow(SolutionDirectory solutionDirectory)
    {
        InitializeComponent();

        var options = GeneralOptions.Instance;

        // What the working copy already knows beats what was typed last time, and the solution
        // name is the last resort for a solution that is not in a working copy yet.
        var remote = GitConfiguration.FindGitHubRepository(solutionDirectory.FullPath);

        RepositoryName.Text = remote?.Name ?? solutionDirectory.Name;
        GitHubUser.Text = remote?.Owner ?? options.DefaultGitHubUser;
        SecurityEmail.Text = options.DefaultSecurityEmail;
    }

    private void CreateFilesButton_Click(object sender, RoutedEventArgs e)
    {
        var options = GeneralOptions.Instance;

        options.DefaultGitHubUser = GitHubUser.Text.Trim();
        options.DefaultSecurityEmail = SecurityEmail.Text.Trim();
        options.Save();

        _projectDetails = new ProjectDetails(
            GitHubUser.Text.Trim(),
            RepositoryName.Text.Trim(),
            NuGetPackage.Text.Trim(),
            SecurityEmail.Text.Trim());

        DialogResult = true;
    }
}
