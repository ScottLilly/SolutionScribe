using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using System.Windows.Forms;

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
        var licenses = LicenseRepository.GetLicenseDetailsList();
        cboLicenseTypes.DataSource = licenses;
        cboLicenseTypes.DisplayMember = "LicenseName";

        tbYears.Text = DateTime.Now.Year.ToString();

        try
        {
            tbCopyrightHolder.Text =
                _settings.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (cboLicenseTypes.SelectedItem is not LicenseDetails selectedLicenseDetails)
        {
            System.Windows.Forms.MessageBox.Show("Please select a license type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            _settings.SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, tbCopyrightHolder.Text);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        PopulatedLicenseText =
            selectedLicenseDetails.PopulateText(tbYears.Text, tbCopyrightHolder.Text);

        DialogResult = DialogResult.OK;
        Close();
    }
}
