using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.master","ExtraIsland·主设置",PackIconKind.BoxAddOutline, PackIconKind.BoxAdd)]
public partial class MainSettingsPage
{
    public MainSettingsPage()
    {
        Settings = GlobalConstants.Handlers.MainConfig!.Data;
        InitializeComponent();
        Settings.PropertyChanged += SettingsOnPropertyChanged;
    }
    public MainConfigData Settings { get; set; }
    private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.IsLifeModeActivated))
        {
            RequestRestart();
        }
    }
}