using Microsoft.VisualStudio.PlatformUI;
using SolutionScribe.Core.Models;
using SolutionScribe.Core.Services;
using System.Windows;
// Community.VisualStudio.Toolkit, which is a global using, has a MessageBox and a
// SelectionChangedEventArgs of its own.
using MessageBox = System.Windows.MessageBox;
using SelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace SolutionScribe.Windows;

public partial class LicenseDataWindow : DialogWindow
{
    private readonly SettingsRepository _settings =
        new SettingsRepository(SettingsRepository.DefaultSettingsFilePath, ex => ex.Log());

    /// <summary>Empty until the user accepts the dialog.</summary>
    private string _populatedLicenseText = string.Empty;

    /// <summary>
    /// Shows the dialog and returns the license text with the year and copyright holder filled in,
    /// or null if the user canceled. Must be called on the UI thread.
    /// </summary>
    internal static string? AskForLicenseText()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dialog = new LicenseDataWindow();

        // ShowModal parents the dialog to the Visual Studio main window and blocks input to the
        // rest of the IDE while it is open, which is what the shell expects of a modal dialog and
        // what the WinForms version needed a helper of its own to do.
        return dialog.ShowModal() == true
            ? dialog._populatedLicenseText.Trim()
            : null;
    }

    public LicenseDataWindow()
    {
        InitializeComponent();

        // Setting the selection raises SelectionChanged, which is what enables or disables the
        // copyright fields for the license that starts out selected.
        LicenseTypes.ItemsSource = LicenseRepository.GetLicenseDetailsList();
        LicenseTypes.SelectedIndex = 0;

        CopyrightYears.Text = DateTime.Now.Year.ToString();

        try
        {
            CopyrightHolder.Text =
                _settings.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder);
        }
        catch (Exception ex)
        {
            ShowError($"Error loading settings: {ex.Message}");
        }
    }

    private void LicenseTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            LicenseTypes.SelectedItem is LicenseDetails license && license.HasPlaceholders;

        CopyrightYearsLabel.IsEnabled = hasPlaceholders;
        CopyrightYears.IsEnabled = hasPlaceholders;
        CopyrightHolderLabel.IsEnabled = hasPlaceholders;
        CopyrightHolder.IsEnabled = hasPlaceholders;

        FixedTextNote.Visibility = hasPlaceholders ? Visibility.Collapsed : Visibility.Visible;
    }

    private void CreateFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (LicenseTypes.SelectedItem is not LicenseDetails selectedLicenseDetails)
        {
            ShowError("Please select a license type.");

            return;
        }

        try
        {
            _settings.SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, CopyrightHolder.Text);
        }
        catch (Exception ex)
        {
            ShowError($"Error saving settings: {ex.Message}");
        }

        _populatedLicenseText =
            selectedLicenseDetails.PopulateText(CopyrightYears.Text, CopyrightHolder.Text);

        // Setting this closes the dialog, because ShowModal shows it with ShowDialog.
        DialogResult = true;
    }

    private void ShowError(string message)
    {
        MessageBox.Show(this, message, "Solution Scribe", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
