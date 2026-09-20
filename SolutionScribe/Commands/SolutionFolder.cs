using System.IO;

namespace SolutionScribe.Commands;

/// <summary>
/// The folder holding the open solution. Every command writes its file here and adds it to the
/// solution's "Solution Items" folder.
/// </summary>
internal sealed class SolutionFolder
{
    private const string SOLUTION_ITEMS_FOLDER_NAME = "Solution Items";

    private readonly EnvDTE.Solution _solution;

    private SolutionFolder(EnvDTE.Solution solution, string fullPath)
    {
        _solution = solution;
        FullPath = fullPath;
    }

    public string FullPath { get; }

    /// <summary>
    /// The open solution's folder, or null when there is no open solution, which it reports itself.
    /// Leaves the caller on the UI thread.
    /// </summary>
    // The package's global "Task" alias is non-generic, so the generic form has to be spelled out.
    public static async System.Threading.Tasks.Task<SolutionFolder?> GetAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var dte = await VS.GetServiceAsync<EnvDTE.DTE, EnvDTE.DTE>();
        var solution = dte?.Solution;

        if (solution == null || string.IsNullOrWhiteSpace(solution.FullName))
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", "No solution is currently open.");

            return null;
        }

        return new SolutionFolder(solution, Path.GetDirectoryName(solution.FullName));
    }

    public string PathOf(string fileName) => Path.Combine(FullPath, fileName);

    public bool Contains(string fileName) => File.Exists(PathOf(fileName));

    /// <summary>Writes the file and adds it to Solution Items. Must be called on the UI thread.</summary>
    public void Write(string fileName, string content)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        string targetPath = PathOf(fileName);

        File.WriteAllText(targetPath, content);

        AddToSolutionItems(fileName, targetPath);
    }

    private void AddToSolutionItems(string fileName, string targetPath)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        foreach (EnvDTE.Project project in _solution.Projects)
        {
            if (project.Name != SOLUTION_ITEMS_FOLDER_NAME)
            {
                continue;
            }

            foreach (EnvDTE.ProjectItem item in project.ProjectItems)
            {
                if (item.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            project.ProjectItems.AddFromFile(targetPath);

            return;
        }

        // If we got here, there is no "Solution Items" folder yet
        var solution2 = (EnvDTE80.Solution2)_solution;
        var solutionFolder = solution2.AddSolutionFolder(SOLUTION_ITEMS_FOLDER_NAME);

        solutionFolder.ProjectItems.AddFromFile(targetPath);
    }
}
