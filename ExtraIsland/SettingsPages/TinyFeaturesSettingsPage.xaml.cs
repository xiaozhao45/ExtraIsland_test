using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using ExtraIsland.TinyFeatures;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.tiny","ExtraIsland·微功能",PackIconKind.CardsOutline,PackIconKind.CardsPlaying)]
public partial class TinyFeaturesSettingsPage {
    public TinyFeaturesSettingsPage() {
        Settings = GlobalConstants.Handlers.MainConfig!.Data.TinyFeatures;
        InitializeComponent();
    }

    public MainConfigData.TinyFeaturesConfig Settings { get; set; }
    
    void MiscSettingsPage_OnUnloaded(object sender,RoutedEventArgs e) {
        
    }
    void DebugShowButton_OnClick(object sender,RoutedEventArgs e) {
        JuniorGuide.Show(true);
    }
}