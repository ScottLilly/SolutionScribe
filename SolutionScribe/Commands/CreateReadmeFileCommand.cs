namespace SolutionScribe;

[Command(PackageIds.CreateReadmeFileCommand)]
internal sealed class CreateReadmeFileCommand : BaseCommand<CreateReadmeFileCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowAsync("Solution Scribe", "Create README file executed.");
    }
}