using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.debug", "ExtraIsland·调试", PackIconKind.WrenchOutline, PackIconKind.Wrench)]
public partial class DebugSettingsPage {
    public DebugSettingsPage() {
        InitializeComponent();
    }
    void ButtonBase_OnClick(object sender,RoutedEventArgs e) {
        StandaloneViews.PopupNotification popup = new StandaloneViews.PopupNotification(350,575,5000) {
            Header = "ExtraIsland·弹出式通知[调试]",
            PackIconControl = new PackIcon {
                Kind = PackIconKind.Wrench,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 25, Width = 25
            },
            Body = new Label {
                Content = "114514",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        popup.Show();
    }
    
    public PopupDebugProps PopProps { get; set; } = new PopupDebugProps();
    
    [GeneratedRegex("[^0-9.-]+")]
    private static partial Regex NumberRegex();
    void TextBoxNumberCheck(object sender,TextCompositionEventArgs e) {
        Regex re = NumberRegex();
        e.Handled = re.IsMatch(e.Text);
    }
}

public class PopupDebugProps {
    public string Height { get; set; } = "350";
    public string Width { get; set; } = "575";
    public string HoldTime { get; set; } = "5000";

    public int GetHeight() {
        return Convert.ToInt32(Height);
    }
    public int GetWidth() {
        return Convert.ToInt32(Width);
    }
    public int GetHoldTime() {
        return Convert.ToInt32(HoldTime);
    }
}