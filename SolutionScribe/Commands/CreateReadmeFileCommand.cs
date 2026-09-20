using SolutionScribe.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateReadmeFileCommand)]
internal sealed class CreateReadmeFileCommand :
    CreateSolutionFileCommandBase<CreateReadmeFileCommand>
{
    protected override string FileName => "README.md";

    protected override string GetContent() =>
        TempateFileRepository.GetReadmeTemplate();
}
