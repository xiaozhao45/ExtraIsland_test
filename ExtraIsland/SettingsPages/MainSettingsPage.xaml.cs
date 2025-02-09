using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows.Media;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.master","ExtraIsland·主设置",PackIconKind.BoxAddOutline,PackIconKind.BoxAdd)]
public partial class MainSettingsPage {
    public MainSettingsPage() {
        Settings = GlobalConstants.Handlers.MainConfig!.Data;
        InitializeComponent();
        if (EiUtils.IsLyricsIslandInstalled()) {
            ((Chip)LyricsStatCard.Switcher!).Background = Brushes.LightSkyBlue;
            ((Chip)LyricsStatCard.Switcher!).Content = "禁用"; 
            ((Chip)LyricsStatCard.Switcher!).Foreground = Brushes.DarkSlateGray;
        } else if (GlobalConstants.Handlers.LyricsIsland == null) {
            ((Chip)LyricsStatCard.Switcher!).Background = Brushes.Gray;
            ((Chip)LyricsStatCard.Switcher!).Content = "未使用";
        } else {
            ((Chip)LyricsStatCard.Switcher!).Background = GlobalConstants.Handlers.LyricsIsland.Status
                ? Brushes.LightGreen 
                : Brushes.IndianRed;
            ((Chip)LyricsStatCard.Switcher!).Content = GlobalConstants.Handlers.LyricsIsland.Status
                ? "正常" 
                : "错误";
            ((Chip)LyricsStatCard.Switcher!).Foreground = GlobalConstants.Handlers.LyricsIsland.Status
                ? Brushes.DarkOliveGreen 
                : Brushes.White;
        }
        Settings.RestartPropertyChanged += SettingsOnPropertyChanged;
    }
    public MainConfigData Settings { get; set; }
    void SettingsOnPropertyChanged() {
        RequestRestart();
    }
}