using SolutionScribe.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateContributingFileCommand)]
internal sealed class CreateContributingFileCommand :
    CreateSolutionFileCommandBase<CreateContributingFileCommand>
{
    protected override string FileName => "CONTRIBUTING.md";

    protected override string GetContent() =>
        TemplateFileRepository.GetContributingTemplate();
}
