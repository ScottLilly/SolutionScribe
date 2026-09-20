using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class TemplateFileRepositoryTests
{
    /// <summary>A repository with no user template folder, so every answer is an embedded default.</summary>
    private static TemplateFileRepository Embedded() => new TemplateFileRepository();

    #region Embedded defaults

    [TestMethod]
    public void GetChangelogTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(Embedded().GetChangelogTemplate(), "# Changelog");
    }

    [TestMethod]
    public void GetCodeOfConductTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(Embedded().GetCodeOfConductTemplate(), "# Code of Conduct");
    }

    [TestMethod]
    public void GetContributingTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(Embedded().GetContributingTemplate(), "# Contributing");
    }

    [TestMethod]
    public void GetReadmeTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(Embedded().GetReadmeTemplate(), "## Project Overview");
    }

    [TestMethod]
    public void GetSecurityTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        string template = Embedded().GetSecurityTemplate();

        StringAssert.Contains(template, "# Security Policy");
        StringAssert.Contains(template, "## Supported Versions");
        StringAssert.Contains(template, "## Reporting a Vulnerability");
    }

    #endregion

    #region GitHub templates

    [TestMethod]
    public void GetBugReportTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(Embedded().GetBugReportTemplate(), "name: Bug report");
    }

    [TestMethod]
    public void GetFeatureRequestTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(Embedded().GetFeatureRequestTemplate(), "name: Feature request");
    }

    [TestMethod]
    public void GetPullRequestTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        string template = Embedded().GetPullRequestTemplate();

        StringAssert.Contains(template, "## Summary");
        StringAssert.Contains(template, "## Linked issue");
        StringAssert.Contains(template, "## Testing");
    }

    /// <summary>
    /// GitHub only reads name, about and labels out of an issue template's YAML front matter, and
    /// only when the file opens with it. A stripped or reordered BOM would break that silently.
    /// </summary>
    [TestMethod]
    [DataRow("bug report")]
    [DataRow("feature request")]
    public void IssueTemplates_Always_StartWithYamlFrontMatter(string which)
    {
        string template = which == "bug report"
            ? Embedded().GetBugReportTemplate()
            : Embedded().GetFeatureRequestTemplate();

        StringAssert.StartsWith(template, "---");
        StringAssert.Contains(template, "about:");
        StringAssert.Contains(template, "labels:");
    }

    #endregion

    #region User templates

    [TestMethod]
    public void GetTemplate_AUserCopyOfTheTemplate_ReturnsItInsteadOfTheEmbeddedOne()
    {
        using var folder = new TemporaryFolder();

        folder.Write("README.md", "My own README template.");

        Assert.AreEqual("My own README template.", folder.Repository().GetReadmeTemplate());
    }

    [TestMethod]
    public void GetTemplate_AUserCopyInTheGitHubSubfolder_ReturnsIt()
    {
        using var folder = new TemporaryFolder();

        folder.Write(@"GitHub\bug_report.md", "My own bug report template.");

        Assert.AreEqual("My own bug report template.", folder.Repository().GetBugReportTemplate());
    }

    [TestMethod]
    public void GetTemplate_NoUserCopyOfThatTemplate_FallsBackToTheEmbeddedOne()
    {
        using var folder = new TemporaryFolder();

        folder.Write("README.md", "My own README template.");

        // Overriding one template must not take the others with it.
        StringAssert.Contains(folder.Repository().GetChangelogTemplate(), "# Changelog");
    }

    [TestMethod]
    public void GetTemplate_ATemplateFolderThatDoesNotExist_FallsBackToTheEmbeddedTemplates()
    {
        string missing = Path.Combine(Path.GetTempPath(), $"SolutionScribeTests-{Guid.NewGuid():N}");

        StringAssert.Contains(new TemplateFileRepository(missing).GetReadmeTemplate(), "## Project Overview");
    }

    [TestMethod]
    public void GetTemplate_NoTemplateFolderSet_ReturnsTheEmbeddedTemplates()
    {
        foreach (string? folder in new[] { null, "", "   " })
        {
            StringAssert.Contains(new TemplateFileRepository(folder).GetReadmeTemplate(), "## Project Overview");
        }
    }

    [TestMethod]
    public void GetTemplate_AFolderWhereTheTemplateShouldBe_FallsBackToTheEmbeddedTemplate()
    {
        using var folder = new TemporaryFolder();

        Directory.CreateDirectory(Path.Combine(folder.Path, "README.md"));

        StringAssert.Contains(folder.Repository().GetReadmeTemplate(), "## Project Overview");
    }

    [TestMethod]
    public void GetTemplate_AnEmptyUserTemplate_ReturnsItRatherThanTheEmbeddedOne()
    {
        // Second-guessing what somebody put in their own template is worse than writing it.
        using var folder = new TemporaryFolder();

        folder.Write("CHANGELOG.md", string.Empty);

        Assert.AreEqual(string.Empty, folder.Repository().GetChangelogTemplate());
    }

    [TestMethod]
    public void GetEmbeddedTemplate_AUserCopyOfTheTemplate_StillReturnsTheEmbeddedOne()
    {
        // This is what the export command writes out, so it has to ignore what is already there.
        using var folder = new TemporaryFolder();

        folder.Write("README.md", "My own README template.");

        StringAssert.Contains(TemplateFileRepository.GetEmbeddedTemplate("README.md"), "## Project Overview");
    }

    [TestMethod]
    public void UserTemplateFolder_AFolderHoldingEnvironmentVariables_IsExpanded()
    {
        var repository = new TemplateFileRepository(@"%AppData%\Solution Scribe\Templates");

        Assert.IsFalse(repository.UserTemplateFolder.Contains("%"));
        StringAssert.StartsWith(repository.UserTemplateFolder,
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    }

    [TestMethod]
    public void TemplatePaths_Always_NameEveryTemplateTheRepositoryCanReturn()
    {
        // The export command writes one file per entry, so a template missing from this list is a
        // template nobody can override.
        Assert.AreEqual(8, TemplateFileRepository.TemplatePaths.Count);

        foreach (string relativePath in TemplateFileRepository.TemplatePaths)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(TemplateFileRepository.GetEmbeddedTemplate(relativePath)),
                $"'{relativePath}' names no embedded resource, so the export command would write an empty file.");
        }
    }

    [TestMethod]
    public void TemplatePaths_Always_AreWhatTheNamedMethodsReturn()
    {
        var byPath = TemplateFileRepository.TemplatePaths
            .Select(TemplateFileRepository.GetEmbeddedTemplate)
            .ToArray();

        CollectionAssert.AreEquivalent(AllTemplates(), byPath);
    }

    #endregion

    #region Placeholders

    private static string[] AllTemplates()
    {
        var templates = Embedded();

        return
        [
            templates.GetChangelogTemplate(),
            templates.GetCodeOfConductTemplate(),
            templates.GetContributingTemplate(),
            templates.GetReadmeTemplate(),
            templates.GetSecurityTemplate(),
            templates.GetBugReportTemplate(),
            templates.GetFeatureRequestTemplate(),
            templates.GetPullRequestTemplate()
        ];
    }

    /// <summary>
    /// Every shipped template has to be fully populated by <see cref="ProjectDetails"/>, in both
    /// project shapes. A placeholder or a marker surviving means a template was written with a
    /// name the substitution does not know, and the user gets it in their file.
    /// </summary>
    [TestMethod]
    [DataRow("")]
    [DataRow("ScottLilly.CSharpExtender")]
    public void AllTemplates_AfterPopulating_LeaveNothingToSubstitute(string nuGetPackage)
    {
        var details = new ProjectDetails("ScottLilly", "SolutionScribe", nuGetPackage, "security@example.com");

        foreach (string template in AllTemplates())
        {
            string populated = details.PopulateText(template);

            foreach (string leftover in new[]
                     {
                         ProjectDetails.GITHUB_USER_PLACEHOLDER,
                         ProjectDetails.REPOSITORY_PLACEHOLDER,
                         ProjectDetails.NUGET_PACKAGE_PLACEHOLDER,
                         ProjectDetails.SECURITY_EMAIL_PLACEHOLDER,
                         "<!--#if",
                         "<!--#endif-->"
                     })
            {
                Assert.IsFalse(populated.Contains(leftover),
                    $"'{leftover}' survived into a populated template.");
            }
        }
    }

    /// <summary>
    /// The templates used to mark their fill-ins as <c>[your-repo-name]</c>, which nothing
    /// substitutes. They now use the same angle bracket form the license texts do.
    /// </summary>
    [TestMethod]
    public void AllTemplates_Always_UseNoLegacySquareBracketPlaceholders()
    {
        foreach (string template in AllTemplates())
        {
            Assert.IsFalse(template.Contains("[your-"),
                "A template still has a '[your-...]' placeholder, which nothing fills in.");
        }
    }

    #endregion

    /// <summary>
    /// An empty string is what a missing or misnamed embedded resource returns, so this catches a
    /// template that was renamed without its resource name being updated.
    /// </summary>
    [TestMethod]
    public void AllTemplates_Always_AreNonEmptyAndDistinct()
    {
        string[] templates = AllTemplates();

        foreach (string template in templates)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(template));
        }

        CollectionAssert.AllItemsAreUnique(templates);
    }

    /// <summary>
    /// A template folder under the temp directory, deleted when the test is done with it.
    /// </summary>
    private sealed class TemporaryFolder : IDisposable
    {
        public TemporaryFolder()
        {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), $"SolutionScribeTests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public TemplateFileRepository Repository() => new TemplateFileRepository(Path);

        public void Write(string relativePath, string content)
        {
            string fullPath = System.IO.Path.Combine(Path, relativePath);

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, content);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, true);
            }
        }
    }
}
