using SolutionScribe.Core.Models;
using SolutionScribe.Windows;

namespace SolutionScribe.Commands;

/// <summary>
/// Fills a template's placeholders in, asking the user for the details the first time a command
/// needs them. A command writing several files asks once and populates them all from the answer,
/// which is why <see cref="ProjectDetails.PopulateText"/> is public rather than hidden behind
/// this.
/// </summary>
internal static class PopulatedTemplate
{
    /// <summary>
    /// The template ready to write, or null if the user canceled the details dialog. A template
    /// with nothing to fill in never asks. Must be called on the UI thread.
    /// </summary>
    public static string? For(string template, SolutionDirectory solutionDirectory)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (!ProjectDetails.HasPlaceholders(template))
        {
            return template;
        }

        return ProjectDetailsWindow.AskForProjectDetails(solutionDirectory)?.PopulateText(template);
    }
}
