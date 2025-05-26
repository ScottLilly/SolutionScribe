using SolutionScribe.Models;
using SolutionScribe.Services;
using System.Windows.Forms;

namespace SolutionScribe.Windows;

public partial class LicenseDataWindow : Form
{
    internal string PopulatedLicenseText { get; private set; }

    public LicenseDataWindow()
    {
        InitializeComponent();
        StartPosition = FormStartPosition.CenterParent;

        Load += LicenseDataWindow_Load;
    }

    private async void LicenseDataWindow_Load(object sender, EventArgs e)
    {
        var licenses = LicenseRepository.GetLicenseDetailsList();
        cboLicenseTypes.DataSource = licenses;
        cboLicenseTypes.DisplayMember = "LicenseName";

        tbYears.Text = DateTime.Now.Year.ToString();

        try
        {
            tbCopyrightHolder.Text =
                await SettingsRepository.GetSettingAsync(SettingsRepository.Key.DefaultCopyrightHolder);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnOK_Click(object sender, EventArgs e)
    {
        LicenseDetails selectedLicenseDetails = cboLicenseTypes.SelectedItem as LicenseDetails;

        if (selectedLicenseDetails == null)
        {
            System.Windows.Forms.MessageBox.Show("Please select a license type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            await SettingsRepository.SaveSettingAsync(SettingsRepository.Key.DefaultCopyrightHolder, tbCopyrightHolder.Text);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        PopulatedLicenseText = selectedLicenseDetails.LicenseText
            .Replace("<year>", tbYears.Text)
            .Replace("<copyright holder>", tbCopyrightHolder.Text);

        DialogResult = DialogResult.OK;
        Close();
    }
}
