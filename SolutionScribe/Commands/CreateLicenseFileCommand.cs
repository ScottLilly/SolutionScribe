namespace SolutionScribe;

[Command(PackageIds.CreateLicenseFileCommand)]
internal sealed class CreateLicenseFileCommand : BaseCommand<CreateLicenseFileCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await VS.MessageBox.ShowAsync("Solution Scribe", "Create LICENSE file executed.");
    }
}