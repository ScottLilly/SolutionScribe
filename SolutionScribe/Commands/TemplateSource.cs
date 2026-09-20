using SolutionScribe.Core.Services;
using SolutionScribe.Options;

namespace SolutionScribe.Commands;

/// <summary>
/// The template repository every command reads from, pointed at whatever template folder the user
/// has set in Tools > Options. Read fresh each time, so editing the setting takes effect without
/// restarting Visual Studio.
/// </summary>
internal static class TemplateSource
{
    public static TemplateFileRepository Current =>
        new TemplateFileRepository(GeneralOptions.Instance.TemplateFolder);
}
