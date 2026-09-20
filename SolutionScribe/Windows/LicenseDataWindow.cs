using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using System.Windows.Forms;
// Community.VisualStudio.Toolkit, which is a global using, has a MessageBox of its own.
using MessageBox = System.Windows.Forms.MessageBox;

namespace SolutionScribe.Windows;

public partial class LicenseDataWindow : Form
{
    private readonly SettingsRepository _settings =
        new SettingsRepository(SettingsRepository.DefaultSettingsFilePath, ex => ex.Log());

    /// <summary>Empty until the user accepts the dialog.</summary>
    internal string PopulatedLicenseText { get; private set; } = string.Empty;

    public LicenseDataWindow()
    {
        InitializeComponent();
        StartPosition = FormStartPosition.CenterParent;

        Load += LicenseDataWindow_Load;
    }

    private void LicenseDataWindow_Load(object sender, EventArgs e)
    {
        cboLicenseTypes.DataSource = LicenseRepository.GetLicenseDetailsList();
        cboLicenseTypes.DisplayMember = nameof(LicenseDetails.LicenseName);
        cboLicenseTypes.SelectedIndexChanged += CboLicenseTypes_SelectedIndexChanged;

        tbYears.Text = DateTime.Now.Year.ToString();

        try
        {
            tbCopyrightHolder.Text =
                _settings.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // The handler is wired after the data source, so the initial selection never raised it.
        ShowCopyrightFieldsForSelectedLicense();
    }

    private void CboLicenseTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        ShowCopyrightFieldsForSelectedLicense();
    }

    /// <summary>
    /// Most of the GNU, Eclipse and Mozilla texts have nowhere to put a year or a copyright holder,
    /// so the fields would silently do nothing for them.
    /// </summary>
    private void ShowCopyrightFieldsForSelectedLicense()
    {
        bool hasPlaceholders =
            cboLicenseTypes.SelectedItem is LicenseDetails license && license.HasPlaceholders;

        lblCopyrightYears.Enabled = hasPlaceholders;
        tbYears.Enabled = hasPlaceholders;
        lblCopyrightHolder.Enabled = hasPlaceholders;
        tbCopyrightHolder.Enabled = hasPlaceholders;

        lblFixedTextNote.Visible = !hasPlaceholders;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (cboLicenseTypes.SelectedItem is not LicenseDetails selectedLicenseDetails)
        {
            MessageBox.Show("Please select a license type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            _settings.SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, tbCopyrightHolder.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        PopulatedLicenseText =
            selectedLicenseDetails.PopulateText(tbYears.Text, tbCopyrightHolder.Text);

        DialogResult = DialogResult.OK;
        Close();
    }
}
