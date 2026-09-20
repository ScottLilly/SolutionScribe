using SolutionScribe.Core.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateChangelogFileCommand)]
internal sealed class CreateChangelogFileCommand :
    CreateSolutionFileCommandBase<CreateChangelogFileCommand>
{
    protected override string FileName => "CHANGELOG.md";

    protected override string? GetContent(SolutionDirectory solutionDirectory)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        return PopulatedTemplate.For(TemplateFileRepository.GetChangelogTemplate(), solutionDirectory);
    }
}
