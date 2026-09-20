using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Models;
using System;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class ProjectDetailsTests
{
    private static ProjectDetails App() =>
        new ProjectDetails("ScottLilly", "SolutionScribe", string.Empty, "security@example.com");

    private static ProjectDetails Package() =>
        new ProjectDetails("ScottLilly", "CSharpExtender", "ScottLilly.CSharpExtender", "security@example.com");

    #region Placeholders

    [TestMethod]
    public void PopulateText_EveryPlaceholder_ReplacesThemAll()
    {
        string populated = Package().PopulateText(
            "<github user>/<repository> ships <nuget package>, report to <security email>");

        Assert.AreEqual(
            "ScottLilly/CSharpExtender ships ScottLilly.CSharpExtender, report to security@example.com",
            populated);
    }

    [TestMethod]
    public void PopulateText_APlaceholderUsedTwice_ReplacesBoth()
    {
        Assert.AreEqual("SolutionScribe SolutionScribe", App().PopulateText("<repository> <repository>"));
    }

    [TestMethod]
    public void PopulateText_TextWithNoPlaceholders_ReturnsItUnchanged()
    {
        Assert.AreEqual("Nothing to fill in here.", App().PopulateText("Nothing to fill in here."));
    }

    [TestMethod]
    public void PopulateText_AnAppWithNoPackage_LeavesThePackagePlaceholderEmpty()
    {
        Assert.AreEqual("package: ", App().PopulateText("package: <nuget package>"));
    }

    [TestMethod]
    public void HasPlaceholders_TextHoldingOne_IsTrue()
    {
        Assert.IsTrue(ProjectDetails.HasPlaceholders("# <repository>"));
        Assert.IsTrue(ProjectDetails.HasPlaceholders("mailto:<security email>"));
    }

    [TestMethod]
    public void HasPlaceholders_TextHoldingOnlyAConditionalBlock_IsTrue()
    {
        // The block still has to be resolved, even with nothing to substitute inside it.
        Assert.IsTrue(ProjectDetails.HasPlaceholders("<!--#if nuget-->\r\nPackage\r\n<!--#endif-->"));
    }

    [TestMethod]
    public void HasPlaceholders_TextHoldingNone_IsFalse()
    {
        Assert.IsFalse(ProjectDetails.HasPlaceholders("# Changelog\r\n\r\nAll notable changes."));
    }

    [TestMethod]
    public void HasPlaceholders_TextHoldingAnInstructionInSquareBrackets_IsFalse()
    {
        // "[Say how to install this project.]" is addressed to whoever edits the file afterwards,
        // not to the extension, and must not make the dialog appear.
        Assert.IsFalse(ProjectDetails.HasPlaceholders("[Say how to install or run this project.]"));
    }

    #endregion

    #region Conditional blocks

    private const string CONDITIONAL_TEMPLATE =
        "Badges\r\n" +
        "<!--#if nuget-->\r\n" +
        "NuGet badge for <nuget package>\r\n" +
        "<!--#endif-->\r\n" +
        "Installation\r\n" +
        "<!--#if app-->\r\n" +
        "Say how to run it.\r\n" +
        "<!--#endif-->\r\n" +
        "<!--#if nuget-->\r\n" +
        "Install-Package <nuget package>\r\n" +
        "<!--#endif-->\r\n" +
        "End";

    [TestMethod]
    public void PopulateText_APackage_KeepsTheNuGetBlocksAndDropsTheAppBlock()
    {
        string populated = Package().PopulateText(CONDITIONAL_TEMPLATE);

        StringAssert.Contains(populated, "NuGet badge for ScottLilly.CSharpExtender");
        StringAssert.Contains(populated, "Install-Package ScottLilly.CSharpExtender");
        Assert.IsFalse(populated.Contains("Say how to run it."));
    }

    [TestMethod]
    public void PopulateText_AnApp_KeepsTheAppBlockAndDropsTheNuGetBlocks()
    {
        string populated = App().PopulateText(CONDITIONAL_TEMPLATE);

        StringAssert.Contains(populated, "Say how to run it.");
        Assert.IsFalse(populated.Contains("NuGet badge"));
        Assert.IsFalse(populated.Contains("Install-Package"));
    }

    [TestMethod]
    public void PopulateText_AnyConditionalTemplate_LeavesNoMarkersBehind()
    {
        foreach (var details in new[] { App(), Package() })
        {
            string populated = details.PopulateText(CONDITIONAL_TEMPLATE);

            Assert.IsFalse(populated.Contains("<!--#if"), "A #if marker survived.");
            Assert.IsFalse(populated.Contains("<!--#endif-->"), "An #endif marker survived.");
        }
    }

    [TestMethod]
    public void PopulateText_TextOutsideTheBlocks_IsAlwaysKept()
    {
        string populated = App().PopulateText(CONDITIONAL_TEMPLATE);

        StringAssert.Contains(populated, "Badges");
        StringAssert.Contains(populated, "Installation");
        StringAssert.Contains(populated, "End");
    }

    [TestMethod]
    public void PopulateText_AConditionItDoesNotRecognize_KeepsTheContentAndDropsTheMarkers()
    {
        // A template is the user's file. Dropping lines because of a typo in a marker is the worse
        // of the two mistakes, so an unknown condition keeps what is inside it.
        string populated = App().PopulateText("<!--#if nugget-->\r\nKeep me\r\n<!--#endif-->");

        Assert.AreEqual("Keep me", populated.Trim());
    }

    [TestMethod]
    public void PopulateText_AnIfWithNoEndif_KeepsTheRestOfTheTemplate()
    {
        string populated = App().PopulateText("Before\r\n<!--#if app-->\r\nAfter");

        StringAssert.Contains(populated, "Before");
        StringAssert.Contains(populated, "After");
    }

    [TestMethod]
    public void PopulateText_MarkersIndentedOrPadded_StillResolvesThem()
    {
        string populated = Package().PopulateText("   <!--#if nuget-->   \r\nKeep me\r\n   <!--#endif-->");

        Assert.AreEqual("Keep me", populated.Trim());
    }

    [TestMethod]
    public void PopulateText_ATemplateWithUnixLineEndings_DoesNotLeaveStrayCarriageReturns()
    {
        string populated = App().PopulateText("one\n<!--#if app-->\ntwo\n<!--#endif-->\nthree");

        Assert.AreEqual($"one{Environment.NewLine}two{Environment.NewLine}three", populated);
    }

    #endregion

    #region Project type

    [TestMethod]
    public void IsNuGetPackage_APackageName_IsTrue()
    {
        Assert.IsTrue(Package().IsNuGetPackage);
    }

    [TestMethod]
    public void IsNuGetPackage_NoPackageName_IsFalse()
    {
        Assert.IsFalse(App().IsNuGetPackage);
        Assert.IsFalse(new ProjectDetails("u", "r", "   ", "e").IsNuGetPackage);
    }

    #endregion
}
