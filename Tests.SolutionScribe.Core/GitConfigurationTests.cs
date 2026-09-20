using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Services;
using System;
using System.IO;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class GitConfigurationTests
{
    private const string CONFIG_WITH_URL =
        "[core]\r\n" +
        "\trepositoryformatversion = 0\r\n" +
        "[remote \"origin\"]\r\n" +
        "\turl = {0}\r\n" +
        "\tfetch = +refs/heads/*:refs/remotes/origin/*\r\n" +
        "[branch \"master\"]\r\n" +
        "\tremote = origin\r\n";

    private static string ConfigFor(string url) => CONFIG_WITH_URL.Replace("{0}", url);

    #region Remote URL forms

    [TestMethod]
    public void ParseGitHubRepository_AnHttpsUrl_ReturnsTheOwnerAndName()
    {
        var repository = GitConfiguration.ParseGitHubRepository(
            ConfigFor("https://github.com/ScottLilly/SolutionScribe.git"));

        Assert.IsNotNull(repository);
        Assert.AreEqual("ScottLilly", repository.Owner);
        Assert.AreEqual("SolutionScribe", repository.Name);
    }

    [TestMethod]
    public void ParseGitHubRepository_AnHttpsUrlWithoutTheGitSuffix_ReturnsTheOwnerAndName()
    {
        var repository = GitConfiguration.ParseGitHubRepository(
            ConfigFor("https://github.com/ScottLilly/SolutionScribe"));

        Assert.IsNotNull(repository);
        Assert.AreEqual("SolutionScribe", repository.Name);
    }

    [TestMethod]
    public void ParseGitHubRepository_AnSshUrl_ReturnsTheOwnerAndName()
    {
        var repository = GitConfiguration.ParseGitHubRepository(
            ConfigFor("git@github.com:ScottLilly/SolutionScribe.git"));

        Assert.IsNotNull(repository);
        Assert.AreEqual("ScottLilly", repository.Owner);
        Assert.AreEqual("SolutionScribe", repository.Name);
    }

    [TestMethod]
    public void ParseGitHubRepository_AnSshProtocolUrl_ReturnsTheOwnerAndName()
    {
        var repository = GitConfiguration.ParseGitHubRepository(
            ConfigFor("ssh://git@github.com/ScottLilly/SolutionScribe.git"));

        Assert.IsNotNull(repository);
        Assert.AreEqual("ScottLilly", repository.Owner);
        Assert.AreEqual("SolutionScribe", repository.Name);
    }

    [TestMethod]
    public void ParseGitHubRepository_AUrlWithATrailingSlash_ReturnsTheOwnerAndName()
    {
        var repository = GitConfiguration.ParseGitHubRepository(
            ConfigFor("https://github.com/ScottLilly/SolutionScribe/"));

        Assert.IsNotNull(repository);
        Assert.AreEqual("SolutionScribe", repository.Name);
    }

    #endregion

    #region Configurations with nothing to find

    [TestMethod]
    public void ParseGitHubRepository_ARemoteThatIsNotGitHub_ReturnsNull()
    {
        Assert.IsNull(GitConfiguration.ParseGitHubRepository(
            ConfigFor("https://dev.azure.com/ScottLilly/_git/SolutionScribe")));
    }

    [TestMethod]
    public void ParseGitHubRepository_NoOriginRemote_ReturnsNull()
    {
        string config =
            "[core]\r\n\trepositoryformatversion = 0\r\n" +
            "[remote \"upstream\"]\r\n\turl = https://github.com/Someone/Else.git\r\n";

        Assert.IsNull(GitConfiguration.ParseGitHubRepository(config));
    }

    [TestMethod]
    public void ParseGitHubRepository_AnOriginWithNoUrl_ReturnsNull()
    {
        Assert.IsNull(GitConfiguration.ParseGitHubRepository(
            "[remote \"origin\"]\r\n\tfetch = +refs/heads/*:refs/remotes/origin/*\r\n"));
    }

    [TestMethod]
    public void ParseGitHubRepository_AGitHubUrlWithNoRepositoryName_ReturnsNull()
    {
        Assert.IsNull(GitConfiguration.ParseGitHubRepository(ConfigFor("https://github.com/ScottLilly")));
    }

    [TestMethod]
    public void ParseGitHubRepository_AnEmptyConfiguration_ReturnsNull()
    {
        Assert.IsNull(GitConfiguration.ParseGitHubRepository(string.Empty));
    }

    [TestMethod]
    public void ParseGitHubRepository_AUrlInAnotherRemoteSection_IsNotMistakenForOrigin()
    {
        string config =
            "[remote \"upstream\"]\r\n\turl = https://github.com/Upstream/Project.git\r\n" +
            "[remote \"origin\"]\r\n\turl = https://github.com/ScottLilly/Fork.git\r\n";

        var repository = GitConfiguration.ParseGitHubRepository(config);

        Assert.IsNotNull(repository);
        Assert.AreEqual("ScottLilly", repository.Owner);
        Assert.AreEqual("Fork", repository.Name);
    }

    #endregion

    #region Finding the working copy

    [TestMethod]
    public void FindGitHubRepository_ADirectoryBelowTheRepositoryRoot_FindsTheConfiguration()
    {
        string root = Path.Combine(Path.GetTempPath(), $"SolutionScribeTests-{Guid.NewGuid():N}");
        string nested = Path.Combine(root, "src", "TheSolution");

        try
        {
            Directory.CreateDirectory(Path.Combine(root, ".git"));
            Directory.CreateDirectory(nested);
            File.WriteAllText(Path.Combine(root, ".git", "config"),
                ConfigFor("https://github.com/ScottLilly/SolutionScribe.git"));

            var repository = GitConfiguration.FindGitHubRepository(nested);

            Assert.IsNotNull(repository);
            Assert.AreEqual("SolutionScribe", repository.Name);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void FindGitHubRepository_ADirectoryInNoWorkingCopy_ReturnsNull()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"SolutionScribeTests-{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(directory);

            Assert.IsNull(GitConfiguration.FindGitHubRepository(directory));
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    #endregion
}
