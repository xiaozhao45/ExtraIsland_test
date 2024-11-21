using System.Windows;
using System.Windows.Controls;
using ClassIsland.Core.Abstractions.Controls;

namespace ExtraIsland.Components;

public partial class FluentClockSettings : ComponentBase<FluentClockConfig> {
    public FluentClockSettings() {
        InitializeComponent();
    }


    void FluentClockSettings_OnLoaded(object sender,RoutedEventArgs e) {
        CheckVisibility();
        Settings.OnUseCiFontSizeChanged += CheckVisibility;
    }

    void CheckVisibility() {
        SmallFontSettingCard.Visibility = Settings.UseCiFontSize!.Value ? Visibility.Collapsed : Visibility.Visible;
    }
}