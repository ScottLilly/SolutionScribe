using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class TemplateFileRepositoryTests
{
    [TestMethod]
    public void GetChangelogTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(TemplateFileRepository.GetChangelogTemplate(), "# Changelog");
    }

    [TestMethod]
    public void GetCodeOfConductTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(TemplateFileRepository.GetCodeOfConductTemplate(), "# Code of Conduct");
    }

    [TestMethod]
    public void GetContributingTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(TemplateFileRepository.GetContributingTemplate(), "# Contributing");
    }

    [TestMethod]
    public void GetReadmeTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(TemplateFileRepository.GetReadmeTemplate(), "## Project Overview");
    }

    [TestMethod]
    public void GetSecurityTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        string template = TemplateFileRepository.GetSecurityTemplate();

        StringAssert.Contains(template, "# Security Policy");
        StringAssert.Contains(template, "## Supported Versions");
        StringAssert.Contains(template, "## Reporting a Vulnerability");
    }

    #region GitHub templates

    [TestMethod]
    public void GetBugReportTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(TemplateFileRepository.GetBugReportTemplate(), "name: Bug report");
    }

    [TestMethod]
    public void GetFeatureRequestTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        StringAssert.Contains(TemplateFileRepository.GetFeatureRequestTemplate(), "name: Feature request");
    }

    [TestMethod]
    public void GetPullRequestTemplate_Always_ReturnsTheEmbeddedTemplate()
    {
        string template = TemplateFileRepository.GetPullRequestTemplate();

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
            ? TemplateFileRepository.GetBugReportTemplate()
            : TemplateFileRepository.GetFeatureRequestTemplate();

        StringAssert.StartsWith(template, "---");
        StringAssert.Contains(template, "about:");
        StringAssert.Contains(template, "labels:");
    }

    #endregion

    #region Placeholders

    private static string[] AllTemplates() =>
    [
        TemplateFileRepository.GetChangelogTemplate(),
        TemplateFileRepository.GetCodeOfConductTemplate(),
        TemplateFileRepository.GetContributingTemplate(),
        TemplateFileRepository.GetReadmeTemplate(),
        TemplateFileRepository.GetSecurityTemplate(),
        TemplateFileRepository.GetBugReportTemplate(),
        TemplateFileRepository.GetFeatureRequestTemplate(),
        TemplateFileRepository.GetPullRequestTemplate()
    ];

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
        string[] templates =
        [
            TemplateFileRepository.GetChangelogTemplate(),
            TemplateFileRepository.GetCodeOfConductTemplate(),
            TemplateFileRepository.GetContributingTemplate(),
            TemplateFileRepository.GetReadmeTemplate(),
            TemplateFileRepository.GetSecurityTemplate(),
            TemplateFileRepository.GetBugReportTemplate(),
            TemplateFileRepository.GetFeatureRequestTemplate(),
            TemplateFileRepository.GetPullRequestTemplate()
        ];

        foreach (string template in templates)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(template));
        }

        CollectionAssert.AllItemsAreUnique(templates);
    }
}
