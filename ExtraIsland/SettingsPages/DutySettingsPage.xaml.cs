using System.Windows.Controls;
using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.duty","ExtraIsland·值日",PackIconKind.UsersOutline,PackIconKind.Users)]
public partial class DutySettingsPage {
    public DutySettingsPage() {
        Settings = GlobalConstants.ConfigHandlers.OnDuty!;
        InitializeComponent();
    }

    public OnDutyPersistedConfig Settings { get; set; }

    void DataGrid_SelectedCellsChanged(object sender,SelectedCellsChangedEventArgs e) {
        Settings.Save();
    }
}