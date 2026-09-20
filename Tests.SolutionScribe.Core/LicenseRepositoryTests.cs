using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
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

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsUniqueSpdxIds()
    {
        var spdxIds = LicenseRepository.GetLicenseDetailsList()
            .Select(license => license.SPDXID)
            .ToList();

        CollectionAssert.AllItemsAreUnique(spdxIds);
    }

    [TestMethod]
    public void GetLicenseDetailsList_Always_ReturnsAnOpenSourceOrgUrlForEveryLicense()
    {
        foreach (var license in LicenseRepository.GetLicenseDetailsList())
        {
            StringAssert.StartsWith(license.LicenseUrl, "https://opensource.org/license/", license.SPDXID);
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
