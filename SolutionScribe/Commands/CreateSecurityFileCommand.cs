using SolutionScribe.Core.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateSecurityFileCommand)]
internal sealed class CreateSecurityFileCommand :
    CreateSolutionFileCommandBase<CreateSecurityFileCommand>
{
    protected override string FileName => "SECURITY.md";

    protected override string GetContent() =>
        TemplateFileRepository.GetSecurityTemplate();
}
