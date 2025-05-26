using SolutionScribe.Services;

namespace SolutionScribe;

[Command(PackageIds.CreateChangelogFileCommand)]
internal sealed class CreateChangelogFileCommand : 
    CreateFileFromTemplateCommandBase<CreateChangelogFileCommand>
{
    protected override string FileName => "CHANGELOG.md";
    protected override string TemplateContent => 
        TempateFileRepository.GetChangelogTemplate();

    protected override async Task OnFileCreatedAsync(string filePath)
    {
        await VS.Documents.OpenAsync(filePath);
    }
}