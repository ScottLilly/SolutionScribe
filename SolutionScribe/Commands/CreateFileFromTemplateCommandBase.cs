using System.IO;

namespace SolutionScribe;

internal abstract class CreateFileFromTemplateCommandBase<T> : BaseCommand<T> where T : BaseCommand<T>, new()
{
    protected abstract string FileName { get; }

    protected abstract string TemplateContent { get; }

    protected virtual Task OnFileCreatedAsync(string filePath)
    {
        return Task.CompletedTask;
    }

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

        try
        {
            File.WriteAllText(targetPath, TemplateContent);

            bool fileAlreadyInSolution = false;

            foreach (EnvDTE.Project project in solution.Projects)
            {
                if (project.Name == "Solution Items")
                {
                    foreach (EnvDTE.ProjectItem item in project.ProjectItems)
                    {
                        if (item.Name.Equals(FileName, StringComparison.OrdinalIgnoreCase))
                        {
                            fileAlreadyInSolution = true;
                            break;
                        }
                    }

                    if (!fileAlreadyInSolution)
                    {
                        project.ProjectItems.AddFromFile(targetPath);
                    }

                    return;
                }
            }

            var solution2 = (EnvDTE80.Solution2)solution;
            var solutionFolder = solution2.AddSolutionFolder("Solution Items");
            solutionFolder.ProjectItems.AddFromFile(targetPath);

            await OnFileCreatedAsync(targetPath);
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create {FileName}: {ex.Message}");
        }
    }
}