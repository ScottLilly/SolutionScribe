using SolutionScribe.Windows;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateLicenseFileCommand)]
internal sealed class CreateLicenseFileCommand :
    CreateSolutionFileCommandBase<CreateLicenseFileCommand>
{
    protected override string FileName => "LICENSE.txt";

    protected override string? GetContent()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        return LicenseDataWindow.AskForLicenseText();
    }
}
