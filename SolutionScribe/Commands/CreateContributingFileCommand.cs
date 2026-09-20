using SolutionScribe.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateContributingFileCommand)]
internal sealed class CreateContributingFileCommand : 
    CreateFileFromTemplateCommandBase<CreateContributingFileCommand>
{
    protected override string FileName => "CONTRIBUTING.md";
    protected override string TemplateContent =>
        TempateFileRepository.GetContributingTemplate();
}