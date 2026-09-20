using System.IO;

namespace SolutionScribe.Commands;

internal abstract class CreateSolutionFileCommandBase<T> : BaseCommand<T> where T : BaseCommand<T>, new()
{
    private const string SOLUTION_ITEMS_FOLDER_NAME = "Solution Items";

    protected abstract string FileName { get; }

    /// <summary>
    /// The content to write, or null if the user canceled. Called on the UI thread.
    /// </summary>
    protected abstract string? GetContent();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var dte = await VS.GetServiceAsync<EnvDTE.DTE, EnvDTE.DTE>();
        var solution = dte?.Solution;

        if (solution == null || string.IsNullOrWhiteSpace(solution.FullName))
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", "No solution is currently open.");
            return;
        }

        string solutionDir = Path.GetDirectoryName(solution.FullName);
        string targetPath = Path.Combine(solutionDir, FileName);

        if (File.Exists(targetPath) &&
            !await VS.MessageBox.ShowConfirmAsync("Solution Scribe",
                $"{FileName} already exists in the solution folder. Replace it?"))
        {
            return;
        }

        string? content = GetContent();

        if (content == null)
        {
            return;
        }

        try
        {
            File.WriteAllText(targetPath, content);

            AddToSolutionItems(solution, targetPath);
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create {FileName}: {ex.Message}");
            return;
        }

        // Every template needs editing before it is any use, so land the user in the file rather
        // than leaving them to find it in Solution Explorer.
        await VS.Documents.OpenAsync(targetPath);
        await VS.StatusBar.ShowMessageAsync($"Solution Scribe created {FileName}.");
    }

    private void AddToSolutionItems(EnvDTE.Solution solution, string targetPath)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        foreach (EnvDTE.Project project in solution.Projects)
        {
            if (project.Name != SOLUTION_ITEMS_FOLDER_NAME)
            {
                continue;
            }

            foreach (EnvDTE.ProjectItem item in project.ProjectItems)
            {
                if (item.Name.Equals(FileName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            project.ProjectItems.AddFromFile(targetPath);

            return;
        }

        // If we got here, there is no "Solution Items" folder yet
        var solution2 = (EnvDTE80.Solution2)solution;
        var solutionFolder = solution2.AddSolutionFolder(SOLUTION_ITEMS_FOLDER_NAME);

        solutionFolder.ProjectItems.AddFromFile(targetPath);
    }
}
