namespace SolutionScribe;

[Command(PackageIds.CreateContributingCommandId)]
internal sealed class CreateContributingCommand : BaseCommand<CreateContributingCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowWarningAsync("CreateContributingCommand", "Button clicked");
    }
}
