using System.Windows;
using System.Windows.Controls;
using ClassIsland.Core.Abstractions.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

public partial class BetterCountdownSettings : ComponentBase<BetterCountdownConfig> {
    public BetterCountdownSettings() {
        InitializeComponent();
    }

    void UpdateConfig() {
        Settings.TargetDate = (SetCalendar.SelectedDate!.Value.Date).ToString("s");
        Settings.Prefix = TBox.Text;
        Settings.Suffix = SBox.Text;
    }

    void LoadConfig() {
        Settings.TargetDate ??= DateTime.Now.ToString("s");
        Settings.Prefix ??= "现在";
        Settings.Suffix ??= "过去了";
        SetCalendar.SelectedDate = Convert.ToDateTime(Settings.TargetDate);
        TBox.Text = Settings.Prefix;
        SBox.Text = Settings.Suffix;
    }
    
    void Calendar_OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e) {
        UpdateConfig();
    }

    void BetterCountdownSettings_OnLoaded(object sender, RoutedEventArgs e) {
        LoadConfig();
    }

    void TBox_OnTextChanged(object sender,TextChangedEventArgs e) {
        UpdateConfig();
    }
}