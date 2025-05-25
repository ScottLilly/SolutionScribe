namespace SolutionScribe;

[Command(PackageIds.CreateChangelogFileCommand)]
internal sealed class CreateChangelogFileCommand : BaseCommand<CreateChangelogFileCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowAsync("Solution Scribe", "Create CHANGELOG file executed.");
    }
}