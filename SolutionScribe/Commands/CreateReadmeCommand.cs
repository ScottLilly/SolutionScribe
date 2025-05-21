namespace SolutionScribe
{
    [Command(PackageIds.CreateReadmeCommandId)]
    internal sealed class CreateReadmeCommand : BaseCommand<CreateReadmeCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("SolutionScribe", "Button clicked");
        }
    }
}
