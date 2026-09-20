namespace SolutionScribe.Commands;

internal abstract class CreateSolutionFileCommandBase<T> : BaseCommand<T> where T : BaseCommand<T>, new()
{
    protected abstract string FileName { get; }

    /// <summary>
    /// The content to write, or null if the user canceled. Called on the UI thread.
    /// </summary>
    protected abstract string? GetContent();

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var solutionFolder = await SolutionFolder.GetAsync();

        if (solutionFolder == null)
        {
            return;
        }

        if (solutionFolder.Contains(FileName) &&
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
            solutionFolder.Write(FileName, content);
        }
        catch (Exception ex)
        {
            await VS.MessageBox.ShowErrorAsync("Solution Scribe", $"Failed to create {FileName}: {ex.Message}");
            return;
        }

        // Every template needs editing before it is any use, so land the user in the file rather
        // than leaving them to find it in Solution Explorer.
        await VS.Documents.OpenAsync(solutionFolder.PathOf(FileName));
        await VS.StatusBar.ShowMessageAsync($"Solution Scribe created {FileName}.");
    }
}
