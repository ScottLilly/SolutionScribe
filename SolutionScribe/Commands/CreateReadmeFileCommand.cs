using SolutionScribe.Services;

namespace SolutionScribe;

[Command(PackageIds.CreateReadmeFileCommand)]
internal sealed class CreateReadmeFileCommand : 
    CreateFileFromTemplateCommandBase<CreateReadmeFileCommand>
{
    protected override string FileName => "README.md";
    protected override string TemplateContent =>
        TempateFileRepository.GetReadmeTemplate();

    protected override async Task OnFileCreatedAsync(string filePath)
    {
        await VS.Documents.OpenAsync(filePath);
    }
}