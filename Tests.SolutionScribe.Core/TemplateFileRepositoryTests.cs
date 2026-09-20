using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            TemplateFileRepository.GetReadmeTemplate()
        ];

        foreach (string template in templates)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(template));
        }

        CollectionAssert.AllItemsAreUnique(templates);
    }
}
