using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Models;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class LicenseDetailsTests
{
    [TestMethod]
    public void PopulateText_TextWithBothPlaceholders_ReplacesBoth()
    {
        var license = LicenseWithText("Copyright (c) <year> <copyright holder>");

        Assert.AreEqual("Copyright (c) 2026 Scott Lilly", license.PopulateText("2026", "Scott Lilly"));
    }

    [TestMethod]
    public void PopulateText_PlaceholderAppearsTwice_ReplacesEveryOccurrence()
    {
        // GPL v3 and LGPL v3 both carry the copyright line twice.
        var license = LicenseWithText("<year> <copyright holder>\n<year> <copyright holder>");

        Assert.AreEqual("2026 Scott Lilly\n2026 Scott Lilly",
            license.PopulateText("2026", "Scott Lilly"));
    }

    [TestMethod]
    public void PopulateText_TextWithNoPlaceholders_ReturnsTheTextUnchanged()
    {
        var license = LicenseWithText("No placeholders here.");

        Assert.AreEqual("No placeholders here.", license.PopulateText("2026", "Scott Lilly"));
    }

    [TestMethod]
    public void PopulateText_EmptyValues_RemovesThePlaceholders()
    {
        var license = LicenseWithText("Copyright (c) <year> <copyright holder>");

        Assert.AreEqual("Copyright (c)  ", license.PopulateText("", ""));
    }

    [TestMethod]
    public void PopulateText_YearRange_KeepsTheRangeIntact()
    {
        var license = LicenseWithText("Copyright (c) <year> <copyright holder>");

        Assert.AreEqual("Copyright (c) 2024-2026 Scott Lilly",
            license.PopulateText("2024-2026", "Scott Lilly"));
    }

    private static LicenseDetails LicenseWithText(string licenseText) =>
        new LicenseDetails("The MIT License", "MIT", "https://opensource.org/license/mit", licenseText);
}
