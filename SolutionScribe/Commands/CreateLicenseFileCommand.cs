using SolutionScribe.Windows;
using System.IO;
using System.Windows.Forms;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateLicenseFileCommand)]
internal sealed class CreateLicenseFileCommand : BaseCommand<CreateLicenseFileCommand>
{
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
        string licenseFilePath = Path.Combine(solutionDir, "LICENSE.txt");

        if (File.Exists(licenseFilePath) &&
            !await VS.MessageBox.ShowConfirmAsync("Solution Scribe",
                "LICENSE.txt already exists in the solution folder. Replace it?"))
        {
            return;
        }

        var details = new LicenseDataWindow();

        if (details.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        await CreateLicenseFileAsync(solution, licenseFilePath, details.PopulatedLicenseText);
    }

    private async Task CreateLicenseFileAsync(EnvDTE.Solution solution, string licenseFilePath, string licenseText)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        try
        {
            // Write the LICENSE.txt file to disk
            File.WriteAllText(licenseFilePath, licenseText.Trim());

            // Check if the file is already part of the solution
            bool fileAlreadyInSolution = false;

            foreach (EnvDTE.Project project in solution.Projects)
            {
                if (project.Name == "Solution Items")
                {
                    foreach (EnvDTE.ProjectItem item in project.ProjectItems)
                    {
                        if (string.Equals(item.Name, "LICENSE.txt", StringComparison.OrdinalIgnoreCase))
                        {
                            fileAlreadyInSolution = true;
                            break;
                        }
                    }

                    if (!fileAlreadyInSolution)
                    {
                        project.ProjectItems.AddFromFile(licenseFilePath);
                    }

                    return;
                }
            }

            // If we got here, there is no "Solution Items" project yet
            var solution2 = (EnvDTE80.Solution2)solution;
            var solutionFolder = solution2.AddSolutionFolder("Solution Items");
            solutionFolder.ProjectItems.AddFromFile(licenseFilePath);
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create LICENSE file: {ex.Message}");
        }
    }
}
