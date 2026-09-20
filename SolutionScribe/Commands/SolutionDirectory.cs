using System.IO;

namespace SolutionScribe.Commands;

/// <summary>
/// The directory holding the open solution. Every command writes its file here, or in a subfolder
/// of it, and adds the file to a solution folder so it is visible in Solution Explorer.
/// </summary>
internal sealed class SolutionDirectory
{
    private const string SOLUTION_ITEMS_FOLDER_NAME = "Solution Items";

    private static readonly char[] s_pathSeparators =
        [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

    private readonly EnvDTE.Solution _solution;

    private SolutionDirectory(EnvDTE.Solution solution, string fullPath, string name)
    {
        _solution = solution;
        FullPath = fullPath;
        Name = name;
    }

    public string FullPath { get; }

    /// <summary>The solution's file name without its extension, which is the default project name.</summary>
    public string Name { get; }

    /// <summary>
    /// The open solution's directory, or null when there is no open solution, which it reports
    /// itself. Leaves the caller on the UI thread.
    /// </summary>
    // The package's global "Task" alias is non-generic, so the generic form has to be spelled out.
    public static async System.Threading.Tasks.Task<SolutionDirectory?> GetAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var dte = await VS.GetServiceAsync<EnvDTE.DTE, EnvDTE.DTE>();
        var solution = dte?.Solution;

        if (solution == null || string.IsNullOrWhiteSpace(solution.FullName))
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", "No solution is currently open.");

            return null;
        }

        return new SolutionDirectory(
            solution,
            Path.GetDirectoryName(solution.FullName),
            Path.GetFileNameWithoutExtension(solution.FullName));
    }

    /// <param name="relativePath">
    /// A file name, or a path below the solution directory such as <c>.github\bug_report.md</c>.
    /// </param>
    public string PathOf(string relativePath) => Path.Combine(FullPath, relativePath);

    public bool Contains(string relativePath) => File.Exists(PathOf(relativePath));

    /// <summary>
    /// Writes the file, creating its folder if needed, and adds it to a solution folder. Must be
    /// called on the UI thread.
    /// </summary>
    public void Write(string relativePath, string content)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        string targetPath = PathOf(relativePath);
        string targetFolder = Path.GetDirectoryName(targetPath);

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        File.WriteAllText(targetPath, content);

        AddToSolutionFolder(relativePath, targetPath);
    }

    private void AddToSolutionFolder(string relativePath, string targetPath)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        string fileName = Path.GetFileName(relativePath);
        var projectItems = FindOrCreateSolutionFolder(SolutionFolderNamesFor(relativePath));

        foreach (EnvDTE.ProjectItem item in projectItems)
        {
            if (item.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        projectItems.AddFromFile(targetPath);
    }

    /// <summary>
    /// A file in the solution directory itself goes in "Solution Items", which is the folder
    /// Visual Studio uses for loose files. A file in a subfolder gets a solution folder per real
    /// folder, so <c>.github\ISSUE_TEMPLATE\bug_report.md</c> nests the same way on disk and in
    /// Solution Explorer.
    /// </summary>
    private static string[] SolutionFolderNamesFor(string relativePath)
    {
        string directory = Path.GetDirectoryName(relativePath);

        return string.IsNullOrEmpty(directory)
            ? [SOLUTION_ITEMS_FOLDER_NAME]
            : directory.Split(s_pathSeparators, StringSplitOptions.RemoveEmptyEntries);
    }

    private EnvDTE.ProjectItems FindOrCreateSolutionFolder(string[] folderNames)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var solution2 = (EnvDTE80.Solution2)_solution;

        var folder = FindTopLevelFolder(folderNames[0]) ?? solution2.AddSolutionFolder(folderNames[0]);

        for (int i = 1; i < folderNames.Length; i++)
        {
            folder = FindNestedFolder(folder, folderNames[i])
                     ?? ((EnvDTE80.SolutionFolder)folder.Object).AddSolutionFolder(folderNames[i]);
        }

        return folder.ProjectItems;
    }

    private EnvDTE.Project? FindTopLevelFolder(string name)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        foreach (EnvDTE.Project project in _solution.Projects)
        {
            if (project.Name == name)
            {
                return project;
            }
        }

        return null;
    }

    private static EnvDTE.Project? FindNestedFolder(EnvDTE.Project parent, string name)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        foreach (EnvDTE.ProjectItem item in parent.ProjectItems)
        {
            if (item.Name == name && item.SubProject != null)
            {
                return item.SubProject;
            }
        }

        return null;
    }
}
