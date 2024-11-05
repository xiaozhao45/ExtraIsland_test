using System.Windows;
using System.Windows.Controls;
using ClassIsland.Core.Abstractions.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

public partial class BetterCountdownSettings : ComponentBase<BetterCountdownConfig> {
    public BetterCountdownSettings() {
        InitializeComponent();
    }
}