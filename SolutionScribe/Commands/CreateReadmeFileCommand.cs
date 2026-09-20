using SolutionScribe.Core.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateReadmeFileCommand)]
internal sealed class CreateReadmeFileCommand :
    CreateSolutionFileCommandBase<CreateReadmeFileCommand>
{
    protected override string FileName => "README.md";

    protected override string? GetContent(SolutionDirectory solutionDirectory)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        return PopulatedTemplate.For(TemplateSource.Current.GetReadmeTemplate(), solutionDirectory);
    }
}
