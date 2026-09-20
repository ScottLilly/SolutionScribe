using System.IO;

namespace SolutionScribe.Core.Services;

public static class TemplateFileRepository
{
    public static string GetChangelogTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.CHANGELOG.md");
    }

    public static string GetCodeOfConductTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.CODE_OF_CONDUCT.md");
    }

    public static string GetContributingTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.CONTRIBUTING.md");
    }

    public static string GetReadmeTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.README.md");
    }

    public static string GetSecurityTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.SECURITY.md");
    }

    public static string GetBugReportTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.GitHub.bug_report.md");
    }

    public static string GetFeatureRequestTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.GitHub.feature_request.md");
    }

    public static string GetPullRequestTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Core.Templates.GitHub.PULL_REQUEST_TEMPLATE.md");
    }

    private static string LoadEmbeddedDefault(string resourceName)
    {
        var assembly = typeof(TemplateFileRepository).Assembly;

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
