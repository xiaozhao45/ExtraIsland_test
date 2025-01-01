using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls.CommonDialog;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.SettingsPages;

[SettingsPageInfo("extraisland.duty", "ExtraIsland·值日", PackIconKind.UsersOutline, PackIconKind.Users)]
public partial class DutySettingsPage {
    public DutySettingsPage() {
        Settings = GlobalConstants.ConfigHandlers.OnDuty!;
        InitializeComponent();
    }
    
    public OnDutyPersistedConfig Settings { get; set; }
}