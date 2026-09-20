using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tests.SolutionScribe.Core;

/// <summary>
/// The VSIX manifest is the one place the extension's version is written. Everything else derives
/// from it, and VSIX Synchronizer, which does the deriving, is a Visual Studio extension that
/// cannot be run from a command line build. So nothing but these tests notices when the generated
/// file is edited by hand or left behind.
/// </summary>
[TestClass]
public class VsixVersionTests
{
    private const string MANIFEST_PATH = @"SolutionScribe\source.extension.vsixmanifest";
    private const string GENERATED_PATH = @"SolutionScribe\source.extension.cs";

    [TestMethod]
    public void ManifestVersion_Always_HasThreeParts()
    {
        // Milestones and releases are named "Version x.y.z", and a two part version here would
        // mean the shipped assembly never matches the release it came from.
        string version = ManifestVersion();

        Assert.IsTrue(
            Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"),
            $"The manifest's Identity version is '{version}', which is not x.y.z.");
    }

    [TestMethod]
    public void GeneratedVsixVersion_Always_MatchesTheManifest()
    {
        Assert.AreEqual(ManifestVersion(), GeneratedVersion(),
            $"{GENERATED_PATH} is out of step with the manifest. Open the solution in Visual " +
            "Studio so VSIX Synchronizer regenerates it, rather than editing the version by hand.");
    }

    private static string ManifestVersion()
    {
        string manifest = File.ReadAllText(Path.Combine(RepositoryRoot(), MANIFEST_PATH));

        return SingleCapture(manifest, "<Identity[^>]*?Version=\"([^\"]+)\"", MANIFEST_PATH);
    }

    private static string GeneratedVersion()
    {
        string generated = File.ReadAllText(Path.Combine(RepositoryRoot(), GENERATED_PATH));

        return SingleCapture(generated, "Version = \"([^\"]+)\"", GENERATED_PATH);
    }

    private static string SingleCapture(string text, string pattern, string forPath)
    {
        var match = Regex.Match(text, pattern);

        Assert.IsTrue(match.Success, $"No version found in {forPath}.");

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
