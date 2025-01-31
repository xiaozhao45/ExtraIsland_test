using System.Windows;
using ClassIsland.Core;
using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using ExtraIsland.TinyFeatures;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.tiny","ExtraIsland·微功能",PackIconKind.CardsOutline,PackIconKind.CardsPlaying)]
public partial class TinyFeaturesSettingsPage {
    public TinyFeaturesSettingsPage() {
        Settings = GlobalConstants.Handlers.MainConfig!.Data.TinyFeatures;
        InitializeComponent();
        dynamic app = AppBase.Current;
        AppSettings = app.Settings;
    }

    public MainConfigData.TinyFeaturesConfig Settings { get; set; }
    
    public dynamic AppSettings { get; set; }
    
    void MiscSettingsPage_OnUnloaded(object sender,RoutedEventArgs e) {
        
    }
    void DebugShowButton_OnClick(object sender,RoutedEventArgs e) {
        JuniorGuide.Show(true);
    }
}