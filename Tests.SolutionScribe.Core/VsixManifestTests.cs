using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tests.SolutionScribe.Core;

/// <summary>
/// The VSIX manifest is the one place the extension's Marketplace metadata is written. Everything
/// else derives from it, and VSIX Synchronizer, which does the deriving, is a Visual Studio
/// extension that cannot be run from a command line build. So nothing but these tests notices when
/// the generated file is edited by hand or left behind.
/// </summary>
[TestClass]
public class VsixManifestTests
{
    private const string MANIFEST_PATH = @"SolutionScribe\source.extension.vsixmanifest";
    private const string GENERATED_PATH = @"SolutionScribe\source.extension.cs";

    #region Version

    [TestMethod]
    public void ManifestVersion_Always_HasThreeParts()
    {
        // Milestones and releases are named "Version x.y.z", and a two part version here would
        // mean the shipped assembly never matches the release it came from.
        string version = ManifestValue("<Identity[^>]*?Version=\"([^\"]+)\"");

        Assert.IsTrue(
            Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"),
            $"The manifest's Identity version is '{version}', which is not x.y.z.");
    }

    [TestMethod]
    public void GeneratedVsixVersion_Always_MatchesTheManifest()
    {
        AssertGeneratedMatchesManifest(
            "Version",
            "<Identity[^>]*?Version=\"([^\"]+)\"");
    }

    #endregion

    #region Marketplace metadata

    [TestMethod]
    public void GeneratedVsixDescription_Always_MatchesTheManifest()
    {
        AssertGeneratedMatchesManifest("Description", "<Description[^>]*>(.*?)</Description>");
    }

    [TestMethod]
    public void GeneratedVsixTags_Always_MatchTheManifest()
    {
        AssertGeneratedMatchesManifest("Tags", "<Tags>(.*?)</Tags>");
    }

    [TestMethod]
    public void Manifest_Always_HasTheMetadataTheMarketplaceListingNeeds()
    {
        string manifest = Manifest();

        foreach (string element in new[] { "MoreInfo", "ReleaseNotes", "Tags", "Icon", "PreviewImage" })
        {
            StringAssert.Contains(manifest, $"<{element}>",
                $"The manifest has no <{element}>, which the Marketplace listing uses.");
        }
    }

    [TestMethod]
    public void ManifestLinks_Always_PointAtTheRepositoryRatherThanAPackagedFile()
    {
        // These three are the elements the schema allows a URL for, and a URL is what they should
        // be: a copy packaged in the VSIX is a second set of documentation to keep current, and
        // nothing in the build notices when it falls behind. <License> is not in this list,
        // because the schema requires that one to be a packaged file.
        foreach (string element in new[] { "MoreInfo", "ReleaseNotes", "GettingStartedGuide" })
        {
            string value = ManifestValue($"<{element}>(.*?)</{element}>");

            StringAssert.StartsWith(value, "https://",
                $"<{element}> is '{value}'. It should be a link to the repository, not a file " +
                "shipped inside the VSIX.");
        }
    }

    #endregion

    #region Installation target

    [TestMethod]
    public void ManifestInstallationTarget_Always_IsOpenEndedFrom17()
    {
        // Visual Studio 2026 decides compatibility from the API version an extension targets. It
        // reads only the lower bound of this range and ignores the upper bound, so an upper bound
        // excludes nothing and only misleads whoever reads it next. 17.0 is what keeps Visual
        // Studio 2022 in.
        string version = ManifestValue("<InstallationTarget[^>]*?Version=\"([^\"]+)\"");

        Assert.AreEqual("[17.0,)", version,
            $"The manifest's installation target is '{version}'. Changing it narrows which " +
            "versions of Visual Studio can install the extension, which is a decision rather " +
            "than a tidy-up.");
    }

    [TestMethod]
    public void Manifest_Always_DeclaresAnX64Payload()
    {
        // Required from 17.0 on, with no default. Visual Studio will not install the extension
        // without it, and the VSIX manifest designer has been known to drop it.
        StringAssert.Contains(Manifest(), "<ProductArchitecture>amd64</ProductArchitecture>",
            "The manifest declares no amd64 product architecture, so Visual Studio will refuse " +
            "to install the extension.");
    }

    #endregion

    private static void AssertGeneratedMatchesManifest(string constantName, string manifestPattern)
    {
        string expected = ManifestValue(manifestPattern);
        string actual = SingleCapture(
            Generated(),
            $"{constantName} = @?\"([^\"]*)\"",
            GENERATED_PATH);

        Assert.AreEqual(expected, actual,
            $"{GENERATED_PATH} is out of step with the manifest on {constantName}. Open the " +
            "solution in Visual Studio so VSIX Synchronizer regenerates it, rather than editing " +
            "it by hand.");
    }

    private static string ManifestValue(string pattern) =>
        SingleCapture(Manifest(), pattern, MANIFEST_PATH);

    private static string Manifest() =>
        File.ReadAllText(Path.Combine(RepositoryRoot(), MANIFEST_PATH));

    private static string Generated() =>
        File.ReadAllText(Path.Combine(RepositoryRoot(), GENERATED_PATH));

    private static string SingleCapture(string text, string pattern, string forPath)
    {
        var match = Regex.Match(text, pattern, RegexOptions.Singleline);

        Assert.IsTrue(match.Success, $"Nothing matching '{pattern}' in {forPath}.");

        return match.Groups[1].Value;
    }

    /// <summary>
    /// Found by walking up from the test assembly rather than by counting folders, so it survives
    /// a change of configuration or target framework.
    /// </summary>
    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "SolutionScribe.sln")))
        {
            directory = directory.Parent;
        }

        Assert.IsNotNull(directory,
            $"No SolutionScribe.sln above '{AppContext.BaseDirectory}', so the repository files " +
            "cannot be checked. These tests have to run from a working copy.");

        return directory.FullName;
    }
}
