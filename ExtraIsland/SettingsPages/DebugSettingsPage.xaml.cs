using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassIsland.Core.Attributes;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.debug", "ExtraIsland·调试", PackIconKind.WrenchOutline, PackIconKind.Wrench)]
public partial class DebugSettingsPage {
    public DebugSettingsPage() {
        InitializeComponent();
    }
    void PopButton_OnClick(object sender,RoutedEventArgs e) {
        StandaloneViews.PopupNotification popup = new StandaloneViews.PopupNotification(350,575,PopProps.GetHoldTime()) {
            Header = "ExtraIsland·弹出式通知[调试]",
            PackIconControl = new PackIcon {
                Kind = PackIconKind.Wrench,
                Height = 25, Width = 25
            },
            Body = new Label {
                Content = "114514",
                FontSize = 50,
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
    
    public JinrishiciData Jinrishici { get; set; } = new JinrishiciData();
    public SainticData Saintic { get; set; } = new SainticData();
    void RequestButton_OnClick(object sender,RoutedEventArgs e) {
        Jinrishici  = JinrishiciData.Fetch();
        Saintic = SainticData.Fetch();
        MessageBox.Show(Jinrishici.Content + "\r\n" + Saintic.ToRhesisData().Content);
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