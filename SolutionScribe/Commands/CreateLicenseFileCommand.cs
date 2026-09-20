using SolutionScribe.Windows;
using System.Windows.Forms;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateLicenseFileCommand)]
internal sealed class CreateLicenseFileCommand :
    CreateSolutionFileCommandBase<CreateLicenseFileCommand>
{
    protected override string FileName => "LICENSE.txt";

    protected override string? GetContent()
    {
        var details = new LicenseDataWindow();

        return details.ShowDialog() == DialogResult.OK
            ? details.PopulatedLicenseText.Trim()
            : null;
    }
}
