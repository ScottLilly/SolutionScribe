namespace SolutionScribe;

[Command(PackageIds.CreateChangeLogCommandId)]
internal sealed class CreateChangeLogCommand : BaseCommand<CreateChangeLogCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowWarningAsync("CreateChangeLogCommand", "Button clicked");
    }
}
