using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.master","ExtraIsland·主设置",PackIconKind.BoxAddOutline, PackIconKind.BoxAdd)]
public partial class MainSettingsPage {
    public MainSettingsPage() {
        Settings = GlobalConstants.Handlers.MainConfig!.Data;
        InitializeComponent();
    }
    public MainConfigData Settings { get; set; }
}