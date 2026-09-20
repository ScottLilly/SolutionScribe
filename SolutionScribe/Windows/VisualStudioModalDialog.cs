using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;

namespace SolutionScribe.Windows;

internal static class VisualStudioModalDialog
{
    /// <summary>
    /// Shows a dialog owned by, and modal to, the Visual Studio shell, so it cannot end up behind
    /// the IDE and no other IDE window accepts input while it is open. Must be called on the UI
    /// thread. Falls back to an unowned dialog if the shell is unavailable.
    /// </summary>
    public static DialogResult Show(Form dialog)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (Package.GetGlobalService(typeof(SVsUIShell)) is not IVsUIShell shell)
        {
            return dialog.ShowDialog();
        }

        shell.GetDialogOwnerHwnd(out IntPtr ownerHandle);
        shell.EnableModeless(0);

        try
        {
            return dialog.ShowDialog(new OwnerWindow(ownerHandle));
        }
        finally
        {
            shell.EnableModeless(1);
        }
    }

    private sealed class OwnerWindow : IWin32Window
    {
        public OwnerWindow(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; }
    }
}
