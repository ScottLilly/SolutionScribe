using SolutionScribe.Windows;
using System.Windows.Forms;

namespace SolutionScribe;

[Command(PackageIds.CreateLicenseFileCommand)]
internal sealed class CreateLicenseFileCommand : BaseCommand<CreateLicenseFileCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        var details = new LicenseDataWindow();

        if (details.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        await CreateLicenseFileAsync(details.PopulatedLicenseText);
    }

    private async Task CreateLicenseFileAsync(string licenseText)
    {
        ;
    }
}