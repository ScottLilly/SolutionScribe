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
}
