namespace SolutionScribe;

[Command(PackageIds.CreateContributingFileCommand)]
internal sealed class CreateContributingFileCommand : BaseCommand<CreateContributingFileCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowAsync("Solution Scribe", "Create CONTRIBUTING file executed.");
    }
}