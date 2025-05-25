namespace SolutionScribe.Commands;

[Command(PackageIds.CreateMITLicenseCommandId)]
internal sealed class CreateMITLicenseCommand : BaseCommand<CreateMITLicenseCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowWarningAsync("CreateMITLicenseCommand", "Button clicked");
    }
}
