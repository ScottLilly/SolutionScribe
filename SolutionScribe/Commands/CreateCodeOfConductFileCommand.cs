using SolutionScribe.Services;

namespace SolutionScribe;

[Command(PackageIds.CreateCodeOfConductFileCommand)]
internal sealed class CreateCodeOfConductFileCommand : 
    CreateFileFromTemplateCommandBase<CreateCodeOfConductFileCommand>
{
    protected override string FileName => "CODE_OF_CONDUCT.md";
    protected override string TemplateContent => 
        TempateFileRepository.GetCodeOfConductTemplate();
}