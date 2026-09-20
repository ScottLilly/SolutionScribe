using SolutionScribe.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateCodeOfConductFileCommand)]
internal sealed class CreateCodeOfConductFileCommand :
    CreateSolutionFileCommandBase<CreateCodeOfConductFileCommand>
{
    protected override string FileName => "CODE_OF_CONDUCT.md";

    protected override string GetContent() =>
        TempateFileRepository.GetCodeOfConductTemplate();
}
