namespace SolutionScribe;

[Command(PackageIds.CreateContributionsCommandId)]
internal sealed class CreateContributionsCommand : BaseCommand<CreateContributionsCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowWarningAsync("CreateContributionsCommand", "Button clicked");
    }
}
