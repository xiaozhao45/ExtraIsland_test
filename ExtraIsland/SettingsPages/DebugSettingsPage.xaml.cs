using System.Windows;
using System.Windows.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.debug", "ExtraIsland·调试", PackIconKind.WrenchOutline, PackIconKind.Wrench)]
public partial class DebugSettingsPage {
    public DebugSettingsPage() {
        InitializeComponent();
    }
    void ButtonBase_OnClick(object sender,RoutedEventArgs e) {
        StandaloneViews.PopupNotification popup = new StandaloneViews.PopupNotification(350,575,5000);
        popup.Show();
    }
}