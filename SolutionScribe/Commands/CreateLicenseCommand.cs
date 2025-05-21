namespace SolutionScribe.Commands;

[Command(PackageIds.CreateLicenseCommandId)]
internal sealed class CreateLicenseCommand : BaseCommand<CreateLicenseCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowWarningAsync("CreateLicenseCommand", "Button clicked");
    }
}
