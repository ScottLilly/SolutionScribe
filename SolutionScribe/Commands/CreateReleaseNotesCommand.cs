namespace SolutionScribe
{
    [Command(PackageIds.CreateReleaseNotesCommandId)]
    internal sealed class CreateReleaseNotesCommand : BaseCommand<CreateReleaseNotesCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("CreateReleaseNotesCommand", "Button clicked");
        }
    }
}
