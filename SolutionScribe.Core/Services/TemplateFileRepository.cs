using System;
using System.Collections.Generic;
using System.IO;

namespace SolutionScribe.Core.Services;

/// <summary>
/// The document templates. Each one is an embedded default that the user can override by putting
/// a file of the same name in their own template folder.
/// </summary>
public class TemplateFileRepository
{
    private const string RESOURCE_PREFIX = "SolutionScribe.Core.Templates.";

    /// <summary>
    /// Every template, as the path it has relative to a template folder. The embedded defaults use
    /// the same layout, so <c>GitHub\bug_report.md</c> is both where the user's copy goes and what
    /// names the embedded resource.
    /// </summary>
    public static IReadOnlyList<string> TemplatePaths { get; } =
    [
        "CHANGELOG.md",
        "CODE_OF_CONDUCT.md",
        "CONTRIBUTING.md",
        "README.md",
        "SECURITY.md",
        @"GitHub\bug_report.md",
        @"GitHub\feature_request.md",
        @"GitHub\PULL_REQUEST_TEMPLATE.md"
    ];

    /// <param name="userTemplateFolder">
    /// Where to look for the user's own copies, or null or empty to use the embedded defaults only.
    /// Environment variables are expanded, so <c>%AppData%\Solution Scribe\Templates</c> works and
    /// survives being exported to another machine through Import and Export Settings.
    /// </param>
    public TemplateFileRepository(string? userTemplateFolder = null)
    {
        string folder = userTemplateFolder?.Trim() ?? string.Empty;

        UserTemplateFolder = folder.Length == 0
            ? string.Empty
            : Environment.ExpandEnvironmentVariables(folder);
    }

    /// <summary>The folder searched for the user's own copies, expanded. Empty when there is none.</summary>
    public string UserTemplateFolder { get; }

    public string GetChangelogTemplate() => GetTemplate("CHANGELOG.md");

    public string GetCodeOfConductTemplate() => GetTemplate("CODE_OF_CONDUCT.md");

    public string GetContributingTemplate() => GetTemplate("CONTRIBUTING.md");

    public string GetReadmeTemplate() => GetTemplate("README.md");

    public string GetSecurityTemplate() => GetTemplate("SECURITY.md");

    public string GetBugReportTemplate() => GetTemplate(@"GitHub\bug_report.md");

    public string GetFeatureRequestTemplate() => GetTemplate(@"GitHub\feature_request.md");

    public string GetPullRequestTemplate() => GetTemplate(@"GitHub\PULL_REQUEST_TEMPLATE.md");

    /// <summary>
    /// The user's copy of a template if there is a readable one, and the embedded default
    /// otherwise. A user file that is there and readable is used as it stands, empty or not,
    /// because second-guessing what somebody put in their own template is worse than writing it.
    /// </summary>
    public string GetTemplate(string relativePath)
    {
        return TryReadUserTemplate(relativePath, out string userTemplate)
            ? userTemplate
            : GetEmbeddedTemplate(relativePath);
    }

    /// <summary>
    /// The shipped default, whatever the user has in their template folder. This is what the
    /// export command writes out as a starting point.
    /// </summary>
    public static string GetEmbeddedTemplate(string relativePath)
    {
        string resourceName = RESOURCE_PREFIX + relativePath.Replace('\\', '.').Replace('/', '.');

        var assembly = typeof(TemplateFileRepository).Assembly;

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private bool TryReadUserTemplate(string relativePath, out string template)
    {
        template = string.Empty;

        if (UserTemplateFolder.Length == 0)
        {
            return false;
        }

        try
        {
            string path = Path.Combine(UserTemplateFolder, relativePath);

            if (!File.Exists(path))
            {
                return false;
            }

            template = File.ReadAllText(path);

            return true;
        }
        catch (Exception ex) when (ex is IOException ||
                                   ex is UnauthorizedAccessException ||
                                   ex is ArgumentException ||
                                   ex is NotSupportedException)
        {
            // A folder that does not exist, a locked file, or a path the platform will not accept
            // all mean the same thing here: there is no usable override, so ship the default
            // rather than failing the command.
            return false;
        }
    }
}
