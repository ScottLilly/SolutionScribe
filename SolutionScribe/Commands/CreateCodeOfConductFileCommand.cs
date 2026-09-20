using SolutionScribe.Core.Services;

namespace SolutionScribe.Commands;

[Command(PackageIds.CreateCodeOfConductFileCommand)]
internal sealed class CreateCodeOfConductFileCommand :
    CreateSolutionFileCommandBase<CreateCodeOfConductFileCommand>
{
    protected override string FileName => "CODE_OF_CONDUCT.md";

    protected override string? GetContent(SolutionDirectory solutionDirectory)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        return PopulatedTemplate.For(TemplateSource.Current.GetCodeOfConductTemplate(), solutionDirectory);
    }
}
