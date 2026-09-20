using System.ComponentModel;

namespace SolutionScribe.Options;

/// <summary>
/// The extension's settings, shown as Tools > Options > Solution Scribe > General and stored in
/// the Visual Studio settings store, so they roam with the user's Visual Studio settings and
/// travel through Import and Export Settings.
/// </summary>
public class GeneralOptions : BaseOptionModel<GeneralOptions>
{
    [Category("Defaults")]
    [DisplayName("Copyright holder")]
    [Description("The name written into the copyright line of a new license file. The license dialog fills this in for you, and remembers what you last used.")]
    [DefaultValue("")]
    public string DefaultCopyrightHolder { get; set; } = string.Empty;

    [Category("Defaults")]
    [DisplayName("GitHub user")]
    [Description("The GitHub account the templates link to. A solution whose git remote points at GitHub uses that instead, so this is the fallback for a solution that is not in a working copy yet.")]
    [DefaultValue("")]
    public string DefaultGitHubUser { get; set; } = string.Empty;

    [Category("Defaults")]
    [DisplayName("Security contact email")]
    [Description("The address SECURITY.md tells people to email about a vulnerability.")]
    [DefaultValue("")]
    public string DefaultSecurityEmail { get; set; } = string.Empty;

    [Category("Templates")]
    [DisplayName("Template folder")]
    [Description("Where Solution Scribe looks for your own copies of the templates. A file here is used instead of the built-in one; anything missing falls back to the built-in. Tools > Solution Scribe > Export templates for editing writes the built-in ones here to start from. Environment variables are expanded. Empty means the built-in templates only.")]
    [DefaultValue(DEFAULT_TEMPLATE_FOLDER)]
    public string TemplateFolder { get; set; } = DEFAULT_TEMPLATE_FOLDER;

    // Left unexpanded so the value means the same thing on another machine when it travels through
    // Import and Export Settings.
    private const string DEFAULT_TEMPLATE_FOLDER = @"%AppData%\Solution Scribe\Templates";
}
