namespace SolutionScribe.Models;

internal class LicenseDetails
{
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
}
