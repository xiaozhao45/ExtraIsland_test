using System.Windows;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ExtraIsland.ConfigHandlers;
using ExtraIsland.Shared;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "B977ECCC-1A59-4C71-A4EB-67780E16E926",
    "值日生",
    PackIconKind.Users,
    "显示值日生姓名,每日轮换"
)]
public partial class OnDuty : ComponentBase {
    public OnDuty() {
        Settings = GlobalConstants.Handlers.OnDuty!;
        InitializeComponent();
        OnOnDutyUpdated();
        Settings.OnDutyUpdated += OnOnDutyUpdated;
    }
    void OnOnDutyUpdated() {
        NameLabel.Content = Settings.PeoplesOnDutyString;
    }
    OnDutyPersistedConfigHandler Settings { get; }
    void OnDuty_OnUnloaded(object sender,RoutedEventArgs e) {
        Settings.OnDutyUpdated -= OnOnDutyUpdated;
    }
}