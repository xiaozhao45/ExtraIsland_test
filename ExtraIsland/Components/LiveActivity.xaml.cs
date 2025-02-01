using System.Windows;
using System.Windows.Media;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ExtraIsland.Shared;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace ExtraIsland.Components;

[ComponentInfo(
    "D61B565D-5BC9-4330-B848-2EDB22F9756E",
    "当前活动",
    PackIconKind.WindowOpen,
    "展示前台窗口标题"
)]
public partial class LiveActivity {
    public LiveActivity(ILessonsService lessonsService) {
        LessonsService = lessonsService;
        InitializeComponent();
        _labelAnimator = new Animators.ClockTransformControlAnimator(CurrentLabel);
    }
    
    ILessonsService LessonsService { get; }
    readonly Animators.ClockTransformControlAnimator _labelAnimator;

    void Check(object? sender,EventArgs eventArgs) {
        Check();
    }
    
    void Check() {
        this.BeginInvoke(() => {
            Icon.Foreground = WindowsUtils.IsOurWindowInForeground() 
                ? Brushes.DeepSkyBlue 
                : Brushes.LightGreen;
            string? title = WindowsUtils.GetActiveWindowTitle();
            if (title == null) {
                CardChip.Visibility = Visibility.Collapsed;
            } else {
                CardChip.Visibility = Visibility.Visible;
                CurrentLabel.Content = title;
            }
        });
    }
    
    void LiveActivity_OnLoaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked += Check;
    }
    void LiveActivity_OnUnloaded(object sender,RoutedEventArgs e) {
        LessonsService.PostMainTimerTicked -= Check;
    }
}