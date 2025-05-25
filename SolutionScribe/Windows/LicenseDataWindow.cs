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

        // Populate input controls
        var licenses = LicenseRepository.GetLicenseDetailsList();
        cboLicenseTypes.DataSource = licenses;
        cboLicenseTypes.DisplayMember = "LicenseName";

        tbYears.Text = DateTime.Now.Year.ToString();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        // TODO : Validate the input fields before closing the dialog
        LicenseDetails selectedLicenseDetails = cboLicenseTypes.SelectedItem as LicenseDetails;

        if (selectedLicenseDetails == null)
        {
            System.Windows.Forms.MessageBox.Show("Please select a license type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // TODO: Populate the license text with the selected license details and user input
        PopulatedLicenseText = selectedLicenseDetails.LicenseText
            .Replace("<year>", tbYears.Text)
            .Replace("<copyright holder>", tbCopyrightHolder.Text);

        Close();
    }
}
