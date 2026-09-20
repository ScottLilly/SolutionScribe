using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using System;
using System.Linq;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class LicenseRepositoryTests
{
    #region Metadata

    /// <summary>
    /// Every embedded license file is offered, and every license offered has a file. Comparing
    /// against the resources rather than a hard-coded count keeps this true as licenses are added.
    /// </summary>
    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsOneEntryPerEmbeddedLicenseFile()
    {
        const string prefix = "SolutionScribe.Core.Licenses.";

        var embeddedSpdxIds = typeof(LicenseRepository).Assembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(prefix) && name.EndsWith(".txt"))
            .Select(name => name.Substring(prefix.Length, name.Length - prefix.Length - ".txt".Length))
            .OrderBy(id => id)
            .ToList();

        var listedSpdxIds = LicenseRepository.GetLicenseDetailsList()
            .Select(license => license.SPDXID)
            .OrderBy(id => id)
            .ToList();

        CollectionAssert.AreEqual(embeddedSpdxIds, listedSpdxIds);
    }

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsUniqueNames()
    {
        var names = LicenseRepository.GetLicenseDetailsList()
            .Select(license => license.LicenseName)
            .ToList();

        CollectionAssert.AllItemsAreUnique(names);
    }

    /// <summary>
    /// The list is what the dialog's drop down shows, in the order it shows it, so a license
    /// added in the wrong place lands in the middle of the list instead of where it is looked for.
    /// </summary>
    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsLicensesSortedByName()
    {
        var names = LicenseRepository.GetLicenseDetailsList()
            .Select(license => license.LicenseName)
            .ToList();

        CollectionAssert.AreEqual(names.OrderBy(name => name, StringComparer.Ordinal).ToList(), names);
    }

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsUniqueSpdxIds()
    {
        var spdxIds = LicenseRepository.GetLicenseDetailsList()
            .Select(license => license.SPDXID)
            .ToList();

        CollectionAssert.AllItemsAreUnique(spdxIds);
    }

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsAnAbsoluteHttpsUrlForEveryLicense()
    {
        foreach (var license in LicenseRepository.GetLicenseDetailsList())
        {
            Assert.IsTrue(
                Uri.TryCreate(license.LicenseUrl, UriKind.Absolute, out Uri uri) &&
                uri.Scheme == Uri.UriSchemeHttps,
                $"{license.SPDXID} has no usable URL: '{license.LicenseUrl}'.");
        }
    }

    /// <summary>
    /// Every license but CC0-1.0 is OSI approved and named after its page there. The OSI rejected
    /// CC0-1.0, so it points at the Creative Commons deed instead.
    /// </summary>
    [TestMethod]
    public void GetLicenseDetailsList_EveryLicenseExceptCC0_LinksToOpenSourceOrg()
    {
        foreach (var license in LicenseRepository.GetLicenseDetailsList())
        {
            string expectedHost = license.SPDXID == "CC0-1.0"
                ? "creativecommons.org"
                : "opensource.org";

            Assert.AreEqual(expectedHost, new Uri(license.LicenseUrl).Host, license.SPDXID);
        }
    }

    #endregion

    #region License text

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsNonEmptyTextForEveryLicense()
    {
        foreach (var license in LicenseRepository.GetLicenseDetailsList())
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(license.LicenseText),
                $"{license.SPDXID} has no embedded text.");
        }
    }

    /// <summary>
    /// Regression test for the GPL v3 command writing the LGPL v3 text. Two licenses sharing a
    /// text means one of them points at the wrong embedded file.
    /// </summary>
    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsADistinctTextForEveryLicense()
    {
        var texts = LicenseRepository.GetLicenseDetailsList()
            .Select(license => license.LicenseText)
            .ToList();

        CollectionAssert.AllItemsAreUnique(texts);
    }

    #endregion

    #region Placeholders

    [TestMethod]
    public void GetLicenseDetailsList_LicenseWithEitherPlaceholder_HasBoth()
    {
        foreach (var license in LicenseRepository.GetLicenseDetailsList())
        {
            bool hasYear = license.LicenseText.Contains(LicenseDetails.YEAR_PLACEHOLDER);
            bool hasHolder = license.LicenseText.Contains(LicenseDetails.COPYRIGHT_HOLDER_PLACEHOLDER);

            Assert.AreEqual(hasYear, hasHolder,
                $"{license.SPDXID} has one placeholder without the other.");
        }
    }

    [TestMethod]
    [DataRow("MIT", true)]
    [DataRow("Apache-2.0", true)]
    [DataRow("BSD-2-Clause", true)]
    [DataRow("BSD-3-Clause", true)]
    [DataRow("AGPL-3.0-only", true)]
    [DataRow("GPL-3.0-only", true)]
    [DataRow("ISC", true)]
    [DataRow("LGPL-3.0-only", true)]
    [DataRow("BSL-1.0", false)]
    [DataRow("CC0-1.0", false)]
    [DataRow("CDDL-1.0", false)]
    [DataRow("EPL-2.0", false)]
    [DataRow("GPL-2.0-only", false)]
    [DataRow("LGPL-2.0-only", false)]
    [DataRow("LGPL-2.1-only", false)]
    [DataRow("MPL-2.0", false)]
    [DataRow("Unlicense", false)]
    public void HasPlaceholders_ForEachLicense_MatchesItsEmbeddedText(string spdxId, bool expected)
    {
        // The dialog disables its year and copyright holder fields from this, so a text edited to
        // add or drop a placeholder has to show up here.
        var license = LicenseRepository.GetLicenseDetailsList()
            .Single(candidate => candidate.SPDXID == spdxId);

        Assert.AreEqual(expected, license.HasPlaceholders);
    }

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsAtLeastOneLicenseWithPlaceholders()
    {
        // Guards against a change that strips the placeholders out of every text, which would
        // leave the dialog's year and copyright holder fields doing nothing.
        var licenses = LicenseRepository.GetLicenseDetailsList();

        Assert.IsTrue(licenses.Any(license =>
            license.LicenseText.Contains(LicenseDetails.YEAR_PLACEHOLDER)));
    }

    #endregion
}
