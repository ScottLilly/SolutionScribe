namespace SolutionScribe.Core.Models;

public class LicenseDetails
{
    public const string YEAR_PLACEHOLDER = "<year>";
    public const string COPYRIGHT_HOLDER_PLACEHOLDER = "<copyright holder>";

    public LicenseDetails(string licenseName, string spdxId, string licenseUrl, string licenseText)
    {
        LicenseName = licenseName;
        SPDXID = spdxId;
        LicenseUrl = licenseUrl;
        LicenseText = licenseText;
    }

    public string LicenseName { get; }
    public string SPDXID { get; }
    public string LicenseUrl { get; }
    public string LicenseText { get; }

    /// <summary>
    /// True when the license text has somewhere to put the year and copyright holder. The GPL,
    /// LGPL, EPL, CDDL and MPL texts are fixed, so neither value is used for them.
    /// </summary>
    public bool HasPlaceholders =>
        LicenseText.Contains(YEAR_PLACEHOLDER) ||
        LicenseText.Contains(COPYRIGHT_HOLDER_PLACEHOLDER);

    /// <summary>
    /// The license text with the year and copyright holder placeholders filled in. A license whose
    /// text has no placeholders is returned unchanged.
    /// </summary>
    public string PopulateText(string years, string copyrightHolder) =>
        LicenseText
            .Replace(YEAR_PLACEHOLDER, years)
            .Replace(COPYRIGHT_HOLDER_PLACEHOLDER, copyrightHolder);
}
