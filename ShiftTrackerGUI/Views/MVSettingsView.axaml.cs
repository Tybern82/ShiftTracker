using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShiftTrackerGUI.Views;

public partial class MVSettingsView : UserControl {


    private static readonly string ZTOtezFont = "https://www.fontspace.com/zt-otez-font-f146082";
    private static readonly string MaragsaFont = "https://www.fontspace.com/maragsa-font-f48694";

    private void OpenURL(string url) {
        try {
            Process.Start(url);
        } catch {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                Process.Start("xdg-open", url);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                Process.Start("open", url);
            } else {
                throw;
            }
        }
    }

    public MVSettingsView() {
        InitializeComponent();

        hlkFontZTOtez.Click += (sender, args) => OpenURL(ZTOtezFont);
        hlkFontMaragsa.Click += (sender, args) => OpenURL(MaragsaFont);
    }
}