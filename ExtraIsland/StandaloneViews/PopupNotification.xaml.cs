using System.Windows;

namespace ExtraIsland.StandaloneViews;

public partial class PopupNotification : Window {
    public PopupNotification() {
        InitializeComponent();
    }
    void ButtonBase_OnClick(object sender,RoutedEventArgs e) {
        Close();
    }
}